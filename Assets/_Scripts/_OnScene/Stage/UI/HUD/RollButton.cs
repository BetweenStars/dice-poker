public class RollButton : BaseStageUI
{
    private void Start()
    {
        Hide();
    }
    
    public override void Hide()
    {
        base.Hide();

        canvasGroup.alpha = 0.5f;
    }
}