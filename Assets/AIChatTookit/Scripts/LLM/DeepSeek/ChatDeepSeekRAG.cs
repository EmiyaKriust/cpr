using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class ChatDeepSeekRAG : ChatDeepSeek
{
    [Header("RAG 配置")]
    [Tooltip("检索的最相关知识点数量")]
    public int topK = 3;
    [Tooltip("知识库文本文件（每行一个知识点）")]
    public TextAsset knowledgeBaseText;
    [Tooltip("DashScope API Key（用于文本向量化）")]
    public string dashscopeApiKey;

    // 本地向量知识库
    private List<KnowledgeItem> embeddedKnowledgeBase = new List<KnowledgeItem>();
    private string embeddingUrl = "https://dashscope.aliyuncs.com/api/v1/services/embeddings/text-embedding/text-embedding";

    IEnumerator Start()
    {
        // 先调用基类 Start（会添加 system 提示词，如果有 m_SystemSetting）
        // 注意：LLM 基类没有 Start，但 ChatDeepSeek 也没有，所以可以不调用 base.Start
        // 但若你希望在开始时添加 system 消息，可以在这里手动添加
        if (!string.IsNullOrEmpty(m_Prompt))
        {
            m_DataList.Add(new SendData("system", m_Prompt));
        }

        if (knowledgeBaseText == null)
            yield break;

        // 尝试从本地缓存加载
        if (LoadKnowledgeFromFile())
            yield break;

        // 没有缓存则向量化知识库
        string[] lines = knowledgeBaseText.text.Split('\n');
        int validCount = 0;
        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            validCount++;
            float[] emb = null;
            yield return StartCoroutine(GetEmbedding(line, true, (e) => emb = e));
            if (emb != null)
            {
                embeddedKnowledgeBase.Add(new KnowledgeItem { text = line, embedding = emb });
                Debug.Log($"已向量化: {line}");
            }
        }

        if (embeddedKnowledgeBase.Count > 0)
        {
            SaveKnowledgeToFile();
            Debug.Log($"知识库向量化完成，共 {embeddedKnowledgeBase.Count} 条");
        }
    }

    // 重写发送消息，先检索再增强
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
            Debug.LogError("问题向量化失败，降级为普通聊天");
            base.PostMsg(_msg, _callback);   // 调用基类 LLM 的 PostMsg（带人物设定）
            yield break;
        }

        // 2. 检索相关知识
        List<KnowledgeItem> relevant = SearchRelevant(queryEmb, topK);

        // 3. 构建增强消息，保留人物设定和语言要求
        string enhancedMsg = BuildEnhancedMessage(_msg, relevant);

        // 4. 将增强消息加入历史（作为 user 消息）
        m_DataList.Add(new SendData("user", enhancedMsg));
        CheckHistory();   // 维持上下文长度

        // 5. 调用 DeepSeek 的 Request
        yield return StartCoroutine(Request(enhancedMsg, _callback));
    }

    // 余弦相似度检索
    private List<KnowledgeItem> SearchRelevant(float[] queryEmb, int k)
    {
        if (embeddedKnowledgeBase.Count == 0)
            return new List<KnowledgeItem>();

        return embeddedKnowledgeBase
            .Select(item => new { item, score = CosineSimilarity(queryEmb, item.embedding) })
            .OrderByDescending(x => x.score)
            .Take(k)
            .Select(x => x.item)
            .ToList();
    }

    private float CosineSimilarity(float[] a, float[] b)
    {
        if (a.Length != b.Length) return 0;
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

    private string BuildEnhancedMessage(string userQuery, List<KnowledgeItem> retrieved)
    {
        string context = retrieved.Count > 0
            ? string.Join("\n", retrieved.Select(i => i.text))
            : "暂无相关已知信息。";

        // 复用基类的人物设定和语言字段
        return $"{m_Prompt}\n回答语言：{lan}\n请严格根据以下已知信息回答问题。如果无法根据已知信息找到答案，请直接说明“这个问题我还不知道”，不要编造内容。\n\n已知信息：\n{context}\n\n问题：{userQuery}";
    }

    // 调用 DashScope 文本向量 API
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
                        Debug.LogError($"Embedding 数据为空: {respText}");
                        callback?.Invoke(null);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"解析 Embedding 响应出错: {e.Message}");
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
        var db = new KnowledgeDB { items = embeddedKnowledgeBase };
        string json = JsonUtility.ToJson(db);
        File.WriteAllText(GetCachePath(), json);
    }

    private bool LoadKnowledgeFromFile()
    {
        string path = GetCachePath();
        if (!File.Exists(path)) return false;

        string json = File.ReadAllText(path);
        var db = JsonUtility.FromJson<KnowledgeDB>(json);
        if (db?.items != null && db.items.Count > 0)
        {
            embeddedKnowledgeBase = db.items;
            Debug.Log($"从缓存加载了 {embeddedKnowledgeBase.Count} 条知识向量");
            return true;
        }
        return false;
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
        public List<KnowledgeItem> items;
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