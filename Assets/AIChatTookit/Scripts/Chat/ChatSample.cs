using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WebGLSupport;

public class ChatSample : MonoBehaviour
{
    public GameObject canvas;

    [SerializeField] private ChatSetting m_ChatSettings;

    #region UI

    [SerializeField] private GameObject m_ChatPanel;

    [SerializeField] public InputField m_InputWord;

    [SerializeField] private Text m_TextBack;

    [SerializeField] private Button m_CommitMsgBtn;

    #endregion


    [SerializeField] private bool m_CreateVoiceMode = false;


    private void Awake()
    {
        m_CommitMsgBtn.onClick.AddListener(delegate { SendData(); });
        DontDestroyOnLoad(canvas);
        InputSettingWhenWebgl();
    }

    #region

    private void InputSettingWhenWebgl()
    {
#if UNITY_WEBGL
        m_InputWord.gameObject.AddComponent<WebGLSupport.WebGLInput>();
#endif
    }

    public void SendData()
    {
        if (m_InputWord.text.Equals(""))
            return;

        m_ChatHistory.Add(m_InputWord.text);

        string _msg = m_InputWord.text;

        m_ChatSettings.m_ChatModel.PostMsg(_msg, CallBack);

        m_InputWord.text = "";
        m_TextBack.text = "Thinking...";
    }

    public void SendData(string _postWord)
    {
        if (_postWord.Equals(""))
            return;

        m_ChatHistory.Add(_postWord);

        string _msg = _postWord;

        m_ChatSettings.m_ChatModel.PostMsg(_msg, CallBack);

        m_InputWord.text = "";
        m_TextBack.text = "Thinking...";
    }

    private void CallBack(string _response)
    {
        _response = _response.Trim();
        m_TextBack.text = "";

        Debug.Log("AI: " + _response);

        m_ChatHistory.Add(_response);

        StartTypeWords(_response);
        return;
    }

    #endregion


    #region

    [SerializeField] private float m_WordWaitTime = 0.03f;

    [SerializeField] private bool m_WriteState = false;

    private void StartTypeWords(string _msg)
    {
        if (_msg == "")
            return;

        m_WriteState = true;
        StartCoroutine(SetTextPerWord(_msg));
    }

    private IEnumerator SetTextPerWord(string _msg)
    {
        int currentPos = 0;
        int charsPerTick = Mathf.Max(1, _msg.Length / 60);

        while (m_WriteState)
        {
            yield return new WaitForSeconds(m_WordWaitTime);
            currentPos += charsPerTick;
            if (currentPos > _msg.Length)
                currentPos = _msg.Length;

            m_TextBack.text = _msg.Substring(0, currentPos);
            m_WriteState = currentPos < _msg.Length;
        }
    }

    #endregion

    #region

    [SerializeField] private List<string> m_ChatHistory;

    [SerializeField] private List<GameObject> m_TempChatBox;

    [SerializeField] private GameObject m_HistoryPanel;

    [SerializeField] private RectTransform m_rootTrans;

    [SerializeField] private ChatPrefab m_PostChatPrefab;

    [SerializeField] private ChatPrefab m_RobotChatPrefab;

    [SerializeField] private ScrollRect m_ScroTectObject;

    public void OpenAndGetHistory()
    {
        m_ChatPanel.SetActive(false);
        m_HistoryPanel.SetActive(true);

        ClearChatBox();
        StartCoroutine(GetHistoryChatInfo());
    }

    public void BackChatMode()
    {
        m_ChatPanel.SetActive(true);
        m_HistoryPanel.SetActive(false);
    }

    private void ClearChatBox()
    {
        while (m_TempChatBox.Count != 0)
        {
            if (m_TempChatBox[0])
            {
                Destroy(m_TempChatBox[0].gameObject);
                m_TempChatBox.RemoveAt(0);
            }
        }
        m_TempChatBox.Clear();
    }

    private IEnumerator GetHistoryChatInfo()
    {
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < m_ChatHistory.Count; i++)
        {
            if (i % 2 == 0)
            {
                ChatPrefab _sendChat = Instantiate(m_PostChatPrefab, m_rootTrans.transform);
                _sendChat.SetText(m_ChatHistory[i]);
                m_TempChatBox.Add(_sendChat.gameObject);
                continue;
            }

            ChatPrefab _reChat = Instantiate(m_RobotChatPrefab, m_rootTrans.transform);
            _reChat.SetText(m_ChatHistory[i]);
            m_TempChatBox.Add(_reChat.gameObject);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(m_rootTrans);
        StartCoroutine(TurnToLastLine());
    }

    private IEnumerator TurnToLastLine()
    {
        yield return new WaitForEndOfFrame();

        m_ScroTectObject.verticalNormalizedPosition = 0;
    }

    #endregion
}
