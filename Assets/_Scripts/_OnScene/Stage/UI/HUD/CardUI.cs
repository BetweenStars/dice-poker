using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using DG.Tweening;
using NUnit.Framework.Constraints;

public class CardUI : BaseStageUI, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    [SerializeField] private RectTransform hideTransform;
    [SerializeField] private RectTransform showTransform;
    private Image cardImage;
    private Vector3 originalScale;
    private float hoverScaleFactor = 1.1f;

    protected override void Awake()
    {
        base.Awake();
        rectTransform = GetComponent<RectTransform>();
        cardImage = GetComponent<Image>();
        originalScale = transform.localScale;
    }

    public void SetCardSprite(Sprite sprite)
    {
        cardImage.sprite = sprite;
    }

    public IEnumerator ShowCoroutine()
    {
        Show();

        //yield return new WaitForSeconds(0.5f);

        Sequence seq = DOTween.Sequence();

        seq.Join(rectTransform.DOAnchorPos(showTransform.anchoredPosition, 0.3f));

        yield return seq.WaitForCompletion();
    }

    public IEnumerator HideCoroutine()
    {
        Sequence seq = DOTween.Sequence();

        seq.Join(rectTransform.DOAnchorPos(hideTransform.anchoredPosition, 0.3f));

        yield return seq.WaitForCompletion();

        Hide();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerEnter != gameObject) return; // Ensure the pointer is actually over this card
        transform.localScale = originalScale * hoverScaleFactor; // Enlarge the card
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale; // Reset to original scale
    }
}
