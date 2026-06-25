using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 免责声明弹窗控制器。
/// 挂在一个包含 Text、Accept 按钮、Cancel 按钮的 Panel 上。
/// </summary>
public class DisclaimerController : MonoBehaviour
{
    private const string PREFS_KEY = "ai_disclaimer_accepted";

    [Header("UI 引用")]
    [SerializeField] private Text m_DisclaimerText;
    [SerializeField] private Button m_AcceptBtn;
    [SerializeField] private Button m_CancelBtn;

    [Header("目标面板（接受后显示）")]
    [SerializeField] private GameObject m_ChatPanel;

    [Header("父级 Canvas（取消时隐藏整个聊天界面）")]
    [SerializeField] private GameObject m_RootCanvas;

    [Header("AI 气泡对象（取消时恢复显示）")]
    [SerializeField] private GameObject m_BubbleObject;

    private void Awake()
    {
        m_AcceptBtn.onClick.AddListener(OnAccept);
        m_CancelBtn.onClick.AddListener(OnCancel);
    }

    private void OnEnable()
    {
        if (PlayerPrefs.GetInt(PREFS_KEY, 0) == 1)
        {
            gameObject.SetActive(false);
            if (m_ChatPanel != null)
                m_ChatPanel.SetActive(true);
        }
    }

    private void OnAccept()
    {
        PlayerPrefs.SetInt(PREFS_KEY, 1);
        PlayerPrefs.Save();

        gameObject.SetActive(false);
        if (m_ChatPanel != null)
            m_ChatPanel.SetActive(true);
    }

    private void OnCancel()
    {
        if (m_RootCanvas != null)
            m_RootCanvas.SetActive(false);

        // 恢复 AI 气泡，让用户下次能再次点击
        if (m_BubbleObject != null)
            m_BubbleObject.SetActive(true);
    }
}
