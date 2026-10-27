using UnityEngine;
using UnityEngine.UI;

public class BetActButton : BaseStageUI
{
    private Button button;

    protected override void Awake()
    {
        base.Awake();

        button = GetComponentInChildren<Button>();
    }

    public override void Hide()
    {
        canvasGroup.alpha = 0.5f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void SetSprite(Sprite sprite) { button.image.sprite = sprite; }
}