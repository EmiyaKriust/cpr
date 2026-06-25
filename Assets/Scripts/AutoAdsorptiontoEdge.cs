using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AutoAdsorptiontoEdge : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // 仅本次运行有效，重启软件后重置
    private static bool _disclaimerAccepted = false;

    private RectTransform popupTransform;
    public GameObject canvasPlane;
    private bool isPopupBeingDragged = false;
    private IEnumerator moveToPosCoroutine = null;

    private Vector2 halfSize;

    private bool hasDragged = false;

    [Header("免责声明与聊天面板")]
    public GameObject disclaimerPanel;
    public GameObject chatPanel;
    public Button disclaimerAcceptBtn;
    public Button disclaimerCancelBtn;

    void Awake()
    {
        popupTransform = (RectTransform)transform;
    }

    void Start()
    {
        halfSize = popupTransform.sizeDelta * 0.5f * popupTransform.root.localScale.x;
        Init();

        if (disclaimerAcceptBtn != null)
            disclaimerAcceptBtn.onClick.AddListener(OnDisclaimerAccept);
        if (disclaimerCancelBtn != null)
            disclaimerCancelBtn.onClick.AddListener(OnDisclaimerCancel);
    }

    public void Init()
    {
        halfSize = popupTransform.sizeDelta * 0.5f * popupTransform.root.localScale.x;
        OnEndDrag(null);
    }

    public void OnPointerClick(PointerEventData data)
    {
        if (!hasDragged)
        {
            if (_disclaimerAccepted)
            {
                // 已接受免责声明：直接打开聊天，隐藏气泡
                OpenChat();
            }
            else
            {
                // 未接受：显示免责声明面板，气泡保持可见
                canvasPlane.SetActive(true);
                if (disclaimerPanel != null)
                    disclaimerPanel.SetActive(true);
                if (chatPanel != null)
                    chatPanel.SetActive(false);
            }
        }
        hasDragged = false;
    }

    private void OnDisclaimerAccept()
    {
        _disclaimerAccepted = true;

        if (disclaimerPanel != null)
            disclaimerPanel.SetActive(false);

        OpenChat();
    }

    private void OnDisclaimerCancel()
    {
        Debug.Log("[Disclaimer] Cancel clicked, hiding panel and canvas");
        if (disclaimerPanel != null)
            disclaimerPanel.SetActive(false);
        canvasPlane.SetActive(false);
    }

    private void OpenChat()
    {
        canvasPlane.SetActive(true);
        if (disclaimerPanel != null)
            disclaimerPanel.SetActive(false);
        if (chatPanel != null)
            chatPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData data)
    {
        isPopupBeingDragged = true;
        hasDragged = true;

        if (moveToPosCoroutine != null)
        {
            StopCoroutine(moveToPosCoroutine);
            moveToPosCoroutine = null;
        }
    }

    public void OnDrag(PointerEventData data)
    {
        popupTransform.position = data.position;
    }

    public void OnEndDrag(PointerEventData data)
    {
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        Vector3 pos = popupTransform.position;

        float distToLeft = pos.x;
        float distToRight = Mathf.Abs(pos.x - screenWidth);

        float distToBottom = Mathf.Abs(pos.y);
        float distToTop = Mathf.Abs(pos.y - screenHeight);

        float horDistance = Mathf.Min(distToLeft, distToRight);
        float vertDistance = Mathf.Min(distToBottom, distToTop);

        if (horDistance < vertDistance)
        {
            if (distToLeft < distToRight)
                pos = new Vector3(halfSize.x, pos.y, 0f);
            else
                pos = new Vector3(screenWidth - halfSize.x, pos.y, 0f);

            pos.y = Mathf.Clamp(pos.y, halfSize.y, screenHeight - halfSize.y);
        }
        else
        {
            if (distToBottom < distToTop)
                pos = new Vector3(pos.x, halfSize.y, 0f);
            else
                pos = new Vector3(pos.x, screenHeight - halfSize.y, 0f);

            pos.x = Mathf.Clamp(pos.x, halfSize.x, screenWidth - halfSize.x);
        }

        if (moveToPosCoroutine != null)
            StopCoroutine(moveToPosCoroutine);

        moveToPosCoroutine = MoveToPosAnimation(pos);
        StartCoroutine(moveToPosCoroutine);

        isPopupBeingDragged = false;
    }

    private IEnumerator MoveToPosAnimation(Vector3 targetPos)
    {
        float modifier = 0f;
        Vector3 initialPos = popupTransform.position;

        while (modifier < 1f)
        {
            modifier += 4f * Time.unscaledDeltaTime;
            popupTransform.position = Vector3.Lerp(initialPos, targetPos, modifier);

            yield return null;
        }
    }
}
