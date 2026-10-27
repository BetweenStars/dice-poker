using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MapScreen : BaseScreen, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region ui-elements
    [SerializeField] private RectTransform mapContent;
    [SerializeField] private RectTransform actualMap;
    [SerializeField] private RectTransform viewport;
    [SerializeField] private MapButton casinoButton;
    [SerializeField] private MapButton barButton;
    // 맵 추가되면 버튼 추가
    #endregion

    [Header("Zoom Settings")]
    public float zoomSpeed = 0.1f;
    public float minZoom = 0.5f;
    public float maxZoom = 2f;

    public override void Show()
    {
        InputManager.Instance.scrollAction.action.performed += OnScroll;

        //ScenarioManager.Instance.CheckAvailableEvents();
        SetMapButtons();

        base.Show();
    }

    public override void Hide()
    {
        InputManager.Instance.scrollAction.action.performed -= OnScroll;
        base.Hide();
    }

    private void OnDisable() { InputManager.Instance.scrollAction.action.performed -= OnScroll; }

    private void ClampMapPosition()
    {
        Vector2 mapSize = actualMap.rect.size;
        Vector2 viewportsize = viewport.rect.size;
        Vector2 scaledMapSize = new Vector2(mapSize.x * mapContent.localScale.x, mapSize.y * mapContent.localScale.y);

        float limitX = Mathf.Max(0, (scaledMapSize.x - viewportsize.x) / 2);
        float limitY = Mathf.Max(0, (scaledMapSize.y - viewportsize.y) / 2);

        Vector2 maxX = new Vector2(limitX, -limitX);
        Vector2 maxY = new Vector2(limitY, -limitY);

        Vector2 clampedPos = mapContent.anchoredPosition;
        clampedPos.x = Mathf.Clamp(clampedPos.x, maxX.y, maxX.x);
        clampedPos.y = Mathf.Clamp(clampedPos.y, maxY.y, maxY.x);

        mapContent.anchoredPosition = clampedPos;
        //Debug.Log($"Clamped Position: {clampedPos}");
    }

    #region Drag
    public void OnBeginDrag(PointerEventData eventData)
    {

    }
    public void OnDrag(PointerEventData eventData)
    {
        mapContent.anchoredPosition += eventData.delta;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        ClampMapPosition();
    }
    #endregion

    #region Scroll
    public void OnScroll(InputAction.CallbackContext context)
    {
        //Debug.Log("Scroll Detected");
        Vector2 scrollDelta = context.ReadValue<Vector2>();

        if (scrollDelta.y == 0) return;

        ZoomAtMouse(scrollDelta.y);
    }
    private void ZoomAtMouse(float scrollAmount)
    {
        float oldScale = mapContent.localScale.x;
        float newScale = Mathf.Clamp(oldScale + scrollAmount * zoomSpeed, minZoom, maxZoom);

        if (Mathf.Approximately(oldScale, newScale)) return;

        Vector2 mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(mapContent, Mouse.current.position.ReadValue(), null, out mousePosition);

        Vector3 beforePosizion = mapContent.TransformPoint(mousePosition);
        mapContent.localScale = Vector3.one * newScale;
        Vector3 afterPosition = mapContent.TransformPoint(mousePosition);
        Vector3 positionDelta = afterPosition - beforePosizion;
        mapContent.anchoredPosition -= new Vector2(positionDelta.x, positionDelta.y);

        ClampMapPosition();
    }
    #endregion

    #region Map Buttons
    public void SetMapButtons()
    {
        ResetMapButtons();

        /*
        foreach (var evt in ScenarioManager.Instance.AvailableStageEventDatas)
        {
            switch (evt.map)
            {
                case MapType.Casino:
                    {
                        if (casinoButton.stageEventData == null) { casinoButton.SetStageEventData(evt); }
                        else { if (evt.priority < casinoButton.stageEventData.priority) casinoButton.SetStageEventData(evt); }
                        break;
                    }
                case MapType.Bar:
                    {
                        if (barButton.stageEventData == null) { barButton.SetStageEventData(evt); }
                        else { if (evt.priority < barButton.stageEventData.priority) barButton.SetStageEventData(evt); }
                        break;
                    }
            }
        }
        */
    }

    private void ResetMapButtons()
    {
        /*
        casinoButton.stageEventData = null;
        barButton.stageEventData = null;
        */
    }
    #endregion
}
