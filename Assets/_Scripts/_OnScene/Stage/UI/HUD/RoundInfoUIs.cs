using TMPro;
using UnityEngine;

public class RoundInfoUIs : BaseStageUI
{
    public TMP_Text roundText;
    public TMP_Text anteText;
    public TMP_Text potText;

    public void UpdateRoundText(int currentRound, int totalRound)
    {
        roundText.text = $"{currentRound}/{totalRound}";
    }
    public void UpdateAnteText(int value)
    {
        anteText.text = $"${value:N0}";
    }
    public void UpdatePotText(int value)
    {
        potText.text = $"${value:N0}";
    }
}
