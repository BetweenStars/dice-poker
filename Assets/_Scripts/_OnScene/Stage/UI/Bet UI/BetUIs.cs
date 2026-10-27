using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BetUIs : BaseStageUI
{
    [SerializeField] private BetActButton callButton;
    [SerializeField] private BetActButton checkButton;
    [SerializeField] private BetActButton foldButton;
    [SerializeField] private BetActButton raiseButton;
    [SerializeField] private BetActButton allInButton;

    public RaiseSlider raiseSlider { get; private set; }

    [SerializeField] private TMP_Text myBetText;
    [SerializeField] private TMP_Text aiBetText;

    protected override void Awake()
    {
        base.Awake();

        raiseSlider = GetComponentInChildren<RaiseSlider>();
    }

    public override void Hide()
    {
        DisableAllButtons();

        raiseSlider.Hide();
    }
    public override void Show()
    {
        raiseSlider.Show();
    }

    public void EnableAvailableButtons(BetAction action)
    {
        if (action.HasFlag(BetAction.Check)) checkButton.Show();
        if (action.HasFlag(BetAction.Call)) callButton.Show();
        if (action.HasFlag(BetAction.Raise)) raiseButton.Show();
        if (action.HasFlag(BetAction.AllIn)) allInButton.Show();
        if (action.HasFlag(BetAction.Fold)) foldButton.Show();
    }
    public void EnableRaiseButton() { raiseButton.Show(); }
    public void DisableRaiseButton() { raiseButton.Hide(); }
    public void DisableAllButtons()
    {
        callButton.Hide();
        checkButton.Hide();
        foldButton.Hide();
        raiseButton.Hide();
        allInButton.Hide();
    }
    public void EnableAllButtons()
    {
        callButton.Show();
        checkButton.Show();
        foldButton.Show();
        raiseButton.Show();
        allInButton.Show();
    }

    public void UpdateBetTexts(int myBet, int aiBet)
    {
        myBetText.text = "My Bet : " + myBet.ToString();
        aiBetText.text = "AI Bet : " + aiBet.ToString();
    }
}
