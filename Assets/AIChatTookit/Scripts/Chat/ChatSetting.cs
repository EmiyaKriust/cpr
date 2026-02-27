using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChatSetting
{
    /// <summary>
    /// 聊天模型
    /// </summary>
    [Header("根据需要挂载不同的llm脚本")]
    [SerializeField] public LLM m_ChatModel;

}
