using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageHUDs : MonoBehaviour
{
    public TMP_Text dice1;
    public TMP_Text dice2;

    public List<CardUI> cardUIs;

    public TMP_Text aiMoneyText;
    public TMP_Text playerMoneyText;

    public RoundInfoUIs roundInfoUi;

    public RollButton rollButton;

    public void UpdatePlayerMoney(int value) => playerMoneyText.text = $"$ {value:N0}";
    public void UpdateAIMoney(int value) => aiMoneyText.text = $"$ {value:N0}";

    public void UpdateDice(int index, int value)
    {
        if (index == 0) { dice1.text = value.ToString(); }
        else if (index == 1) { dice2.text = value.ToString(); }
        else
        {
            Debug.LogWarning("Dice Out of index. index : " + index);
        }
    }

    public void UpdateCardUI(int index, Sprite cardSprite = null)
    {
        if (index == -1)
        {
            foreach (var cardUi in cardUIs)
            {
                StartCoroutine(cardUi.HideCoroutine());
            }

            return;
        }

        cardUIs[index].SetCardSprite(cardSprite);

        StartCoroutine(cardUIs[index].ShowCoroutine());
    }
}
