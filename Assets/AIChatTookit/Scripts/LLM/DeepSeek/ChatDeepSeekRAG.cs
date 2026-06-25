using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class ChatDeepSeekRAG : ChatDeepSeek
{
    [Header("RAG 设置")]
    [Tooltip("检索时返回的最相关知识点数量（每个知识库各取 topK 条）")]
    public int topK = 3;
    [Tooltip("最低相似度阈值（0~1）。最佳匹配低于此值时，拒答并提示知识库无此内容")]
    public float minSimilarityThreshold = 0.35f;
    [Tooltip("低置信度阈值（0~1）。最佳匹配低于此值但高于最低阈值时，附加'仅供参考'提示")]
    public float lowConfidenceThreshold = 0.50f;
    [Tooltip("知识库文本文件（英文），每行一个知识点")]
    public TextAsset knowledgeBaseText;
    [Tooltip("知识库文本文件（中文），每行一个知识点")]
    public TextAsset knowledgeBaseTextZh;
    [Tooltip("DashScope API Key，用于文本向量化")]
    public string dashscopeApiKey;

    // 本地缓存的知识库（英文 + 中文）
    private List<KnowledgeItem> embeddedKnowledgeBase = new List<KnowledgeItem>();
    private List<KnowledgeItem> embeddedKnowledgeBaseZh = new List<KnowledgeItem>();
    private string embeddingUrl = "https://dashscope.aliyuncs.com/api/v1/services/embeddings/text-embedding/text-embedding";

    IEnumerator Start()
    {
        if (!string.IsNullOrEmpty(m_Prompt))
        {
            m_DataList.Add(new SendData("system", m_Prompt));
        }

        if (knowledgeBaseText == null && knowledgeBaseTextZh == null)
            yield break;

        // 优先从本地缓存加载
        if (LoadKnowledgeFromFile())
            yield break;

        // 没有缓存则向量化英文知识库
        if (knowledgeBaseText != null)
        {
            string[] lines = knowledgeBaseText.text.Split('\n');
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                float[] emb = null;
                yield return StartCoroutine(GetEmbedding(line, true, (e) => emb = e));
                if (emb != null)
                {
                    embeddedKnowledgeBase.Add(new KnowledgeItem { text = line, embedding = emb });
                    Debug.Log($"[EN] 向量化完成: {line.Substring(0, Math.Min(line.Length, 60))}...");
                }
            }
            Debug.Log($"英文知识库向量化完成，共 {embeddedKnowledgeBase.Count} 条");
        }

        // 向量化中文知识库
        if (knowledgeBaseTextZh != null)
        {
            string[] lines = knowledgeBaseTextZh.text.Split('\n');
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                float[] emb = null;
                yield return StartCoroutine(GetEmbedding(line, true, (e) => emb = e));
                if (emb != null)
                {
                    embeddedKnowledgeBaseZh.Add(new KnowledgeItem { text = line, embedding = emb });
                    Debug.Log($"[ZH] 向量化完成: {line.Substring(0, Math.Min(line.Length, 60))}...");
                }
            }
            Debug.Log($"中文知识库向量化完成，共 {embeddedKnowledgeBaseZh.Count} 条");
        }

        if (embeddedKnowledgeBase.Count > 0 || embeddedKnowledgeBaseZh.Count > 0)
        {
            SaveKnowledgeToFile();
            Debug.Log($"双知识库向量化全部完成: EN={embeddedKnowledgeBase.Count}条, ZH={embeddedKnowledgeBaseZh.Count}条");
        }
    }

    // 重写发送消息，先检索增强
    public override void PostMsg(string _msg, Action<string> _callback)
    {
        StartCoroutine(PostMsgWithRAG(_msg, _callback));
    }

    private IEnumerator PostMsgWithRAG(string _msg, Action<string> _callback)
    {
        // 1. 向量化用户问题
        float[] queryEmb = null;
        yield return StartCoroutine(GetEmbedding(_msg, false, (e) => queryEmb = e));

        if (queryEmb == null)
        {
            Debug.LogError("查询向量化失败，降级为普通对话");
            base.PostMsg(_msg, _callback);
            yield break;
        }

        // 2. 检索两个知识库，带相似度分数
        List<KnowledgeItem> relevantEn = SearchRelevant(queryEmb, embeddedKnowledgeBase, topK);
        List<KnowledgeItem> relevantZh = SearchRelevant(queryEmb, embeddedKnowledgeBaseZh, topK);

        // 3. 合并去重，按相似度排序
        List<KnowledgeItem> allRelevant = new List<KnowledgeItem>();
        allRelevant.AddRange(relevantEn);
        allRelevant.AddRange(relevantZh);
        allRelevant = allRelevant
            .OrderByDescending(item => CosineSimilarity(queryEmb, item.embedding))
            .Take(topK * 2)
            .ToList();

        // 4. 计算最佳匹配的相似度
        float bestScore = 0f;
        foreach (var item in allRelevant)
        {
            float s = CosineSimilarity(queryEmb, item.embedding);
            if (s > bestScore) bestScore = s;
        }

        Debug.Log($"[RAG] 检索到 {allRelevant.Count} 条相关知识点，最佳相似度={bestScore:F3}");

        // 5. 置信度判断——三级分流
        if (allRelevant.Count == 0 || bestScore < minSimilarityThreshold)
        {
            // 无法匹配：不调 LLM，直接返回拒答提示
            string rejectMsg = GetRejectMessage();
            Debug.Log($"[RAG] 置信度过低（{bestScore:F3} < {minSimilarityThreshold}），拒答");
            m_DataList.Add(new SendData("user", _msg));
            m_DataList.Add(new SendData("assistant", rejectMsg));
            CheckHistory();
            _callback(rejectMsg);
            yield break;
        }

        // 6. 构建增强消息
        bool isLowConfidence = bestScore < lowConfidenceThreshold;
        string enhancedMsg = BuildEnhancedMessage(_msg, allRelevant, isLowConfidence);

        // 7. 将增强消息加入历史记录
        m_DataList.Add(new SendData("user", enhancedMsg));
        CheckHistory();

        // 8. 调用 DeepSeek
        yield return StartCoroutine(Request(enhancedMsg, _callback));
    }

    // 余弦相似度检索
    private List<KnowledgeItem> SearchRelevant(float[] queryEmb, List<KnowledgeItem> kb, int k)
    {
        if (kb == null || kb.Count == 0)
            return new List<KnowledgeItem>();

        return kb
            .Select(item => new { item, score = CosineSimilarity(queryEmb, item.embedding) })
            .OrderByDescending(x => x.score)
            .Take(k)
            .Select(x => x.item)
            .ToList();
    }

    private float CosineSimilarity(float[] a, float[] b)
    {
        if (a == null || b == null || a.Length != b.Length) return 0;
        float dot = 0, magA = 0, magB = 0;
        for (int i = 0; i < a.Length; i++)
        {
            dot += a[i] * b[i];
            magA += a[i] * a[i];
            magB += b[i] * b[i];
        }
        magA = Mathf.Sqrt(magA);
        magB = Mathf.Sqrt(magB);
        return (magA == 0 || magB == 0) ? 0 : dot / (magA * magB);
    }

    private string BuildEnhancedMessage(string userQuery, List<KnowledgeItem> retrieved, bool isLowConfidence = false)
    {
        string context = retrieved.Count > 0
            ? string.Join("\n", retrieved.Select(i => i.text))
            : "未检索到相关信息。";

        string lowConfWarning = isLowConfidence
            ? "\n【提醒】以下已知信息与用户问题的匹配度较低，答案仅供参考，请谨慎判断其可靠性。\n"
            : "";

        // 自动检测用户语言：中文提问用中文回答，英文提问用英文回答
        string responseLan = DetectLanguage(userQuery);

        return $"{m_Prompt}\n回答语言：{responseLan}\n请严格根据以下已知信息回答问题。如果无法从已知信息中找到答案，请直接说明根据当前已知知识库无法回答，不要自行编造内容。\n{lowConfWarning}\n已知信息：\n{context}\n\n问题：{userQuery}";
    }

    /// <summary>
    /// 检测文本是否为中文（含中文字符则为中文，否则为英文）
    /// </summary>
    private string DetectLanguage(string text)
    {
        foreach (char c in text)
        {
            if (c >= 0x4E00 && c <= 0x9FFF)
                return "中文";
        }
        return "English";
    }

    /// <summary>
    /// 知识库无法匹配时返回的标准拒答提示（含免责声明）
    /// </summary>
    private string GetRejectMessage()
    {
        return "We apologise, but this information is not currently available in the knowledge base. We recommend consulting authoritative first-aid guidelines or seeking advice from a healthcare professional.";
    }

    // 调用 DashScope 文本向量化 API
    private IEnumerator GetEmbedding(string text, bool isDocument, Action<float[]> callback)
    {
        var reqBody = new EmbeddingRequest
        {
            model = "text-embedding-v2",
            input = new InputData { texts = new[] { text } },
            parameters = new Parameters { text_type = isDocument ? "document" : "query" }
        };

        string json = JsonUtility.ToJson(reqBody);
        Debug.Log($"[Embedding] 请求: {json}");

        using (UnityWebRequest request = new UnityWebRequest(embeddingUrl, "POST"))
        {
            byte[] body = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(body);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {dashscopeApiKey}");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string respText = request.downloadHandler.text;
                Debug.Log($"[Embedding] 响应: {respText}");
                try
                {
                    var resp = JsonUtility.FromJson<EmbeddingResponse>(respText);
                    if (resp?.output?.embeddings != null && resp.output.embeddings.Length > 0)
                    {
                        callback?.Invoke(resp.output.embeddings[0].embedding);
                    }
                    else
                    {
                        Debug.LogError($"Embedding 返回为空: {respText}");
                        callback?.Invoke(null);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"解析 Embedding 响应异常: {e.Message}");
                    callback?.Invoke(null);
                }
            }
            else
            {
                Debug.LogError($"Embedding 请求失败: {request.responseCode}\n{request.downloadHandler.text}");
                callback?.Invoke(null);
            }
        }
    }

    // --- 本地缓存 ---
    private string GetCachePath() => Path.Combine(Application.persistentDataPath, "knowledge_cache.json");

    private void SaveKnowledgeToFile()
    {
        var db = new KnowledgeDB
        {
            items = embeddedKnowledgeBase,
            itemsZh = embeddedKnowledgeBaseZh
        };
        string json = JsonUtility.ToJson(db);
        File.WriteAllText(GetCachePath(), json);
    }

    private bool LoadKnowledgeFromFile()
    {
        string path = GetCachePath();
        if (!File.Exists(path)) return false;

        string json = File.ReadAllText(path);
        var db = JsonUtility.FromJson<KnowledgeDB>(json);
        if (db == null) return false;

        bool loaded = false;
        if (db.items != null && db.items.Count > 0)
        {
            embeddedKnowledgeBase = db.items;
            Debug.Log($"从缓存加载英文知识库 {embeddedKnowledgeBase.Count} 条");
            loaded = true;
        }
        if (db.itemsZh != null && db.itemsZh.Count > 0)
        {
            embeddedKnowledgeBaseZh = db.itemsZh;
            Debug.Log($"从缓存加载中文知识库 {embeddedKnowledgeBaseZh.Count} 条");
            loaded = true;
        }
        return loaded;
    }

    #region 内部数据结构
    [Serializable]
    private class KnowledgeItem
    {
        public string text;
        public float[] embedding;
    }

    [Serializable]
    private class KnowledgeDB
    {
        public List<KnowledgeItem> items;      // 英文知识库
        public List<KnowledgeItem> itemsZh;    // 中文知识库
    }

    [Serializable]
    private class EmbeddingRequest
    {
        public string model;
        public InputData input;
        public Parameters parameters;
    }

    [Serializable]
    private class InputData
    {
        public string[] texts;
    }

    [Serializable]
    private class Parameters
    {
        public string text_type;
    }

    [Serializable]
    private class EmbeddingResponse
    {
        public OutputData output;
    }

    [Serializable]
    private class OutputData
    {
        public EmbeddingItem[] embeddings;
    }

    [Serializable]
    private class EmbeddingItem
    {
        public int text_index;
        public float[] embedding;
    }
    #endregion
}
