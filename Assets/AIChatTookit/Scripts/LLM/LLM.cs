using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class LLM : MonoBehaviour
{
    [SerializeField] protected string url;

    [Header("发送给模型的提示词设定")]
    [SerializeField] protected string m_Prompt = string.Empty;

    [Header("限定回复语言（设为空则由系统自动检测）")]
    [SerializeField] protected string lan = "";

    [Header("保留的历史条数")]
    [SerializeField] protected int m_HistoryKeepCount = 15;

    [SerializeField] public List<SendData> m_DataList = new List<SendData>();

    [SerializeField] protected Stopwatch stopwatch = new Stopwatch();

    /// <summary>
    /// 根据用户输入自动检测语言：含中文字符则用中文回答，否则用英文
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

    public virtual void PostMsg(string _msg, Action<string> _callback)
    {
        CheckHistory();

        // 如果 lan 为空则自动检测，否则使用手动指定的语言
        string responseLan = string.IsNullOrEmpty(lan) ? DetectLanguage(_msg) : lan;

        string message = "当前为角色扮演设定：" + m_Prompt +
            " 回答的语言：" + responseLan +
            " 以下是我提出的问题：" + _msg;

        m_DataList.Add(new SendData("user", message));

        StartCoroutine(Request(message, _callback));
    }

    public virtual IEnumerator Request(string _postWord, Action<string> _callback)
    {
        yield return new WaitForEndOfFrame();
    }

    public virtual void CheckHistory()
    {
        if (m_DataList.Count > m_HistoryKeepCount)
        {
            m_DataList.RemoveAt(0);
        }
    }

    [Serializable]
    public class SendData
    {
        [SerializeField] public string role;
        [SerializeField] public string content;

        public SendData() { }

        public SendData(string _role, string _content)
        {
            role = _role;
            content = _content;
        }
    }
}
