using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class AutoAdsorptiontoEdge : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    private RectTransform popupTransform;
    public GameObject canvasPlane;
    private bool isPopupBeingDragged = false;
    private IEnumerator moveToPosCoroutine = null;

    private Vector2 halfSize;

    // 新增标志位，用于判断是否发生了拖动
    private bool hasDragged = false;

    void Awake()
    {
        popupTransform = (RectTransform)transform;
    }

    void Start()
    {
        halfSize = popupTransform.sizeDelta * 0.5f * popupTransform.root.localScale.x;
        Init();
    }

    public void Init()
    {
        halfSize = popupTransform.sizeDelta * 0.5f * popupTransform.root.localScale.x;
        OnEndDrag(null);
    }

    public void OnPointerClick(PointerEventData data)
    {
        // 只有在没有拖动的情况下才处理点击事件
        if (!hasDragged)
        {
            canvasPlane.SetActive(true);
            this.gameObject.SetActive(false);
        }
        // 重置拖动标志位
        hasDragged = false;
    }

    public void OnBeginDrag(PointerEventData data)
    {
        isPopupBeingDragged = true;
        hasDragged = true; // 开始拖动，设置标志位

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