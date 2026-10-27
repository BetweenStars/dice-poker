using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    public int m_id = 0;
    public int money;
    public List<CardData> cardsInHand = new List<CardData>();
    public HandResult handResult = new();

    protected bool isActionSelected;
    public BetAction selectedBetAction { get; protected set; }

    public BetAction availableBetAction;
    public int currentBet;
    public int needMoney;

    #region Card
    public void AddCard(CardData cardData)
    {
        if (cardsInHand.Count < 3)
        {
            cardsInHand.Add(cardData);
        }

        if (this is not AIPlayer) { StageUIManager.Instance.UpdateCardUI(cardsInHand.Count - 1, cardData.sprite); }
    }
    #endregion

    public void ResetCards()
    {
        cardsInHand.Clear();
        StageUIManager.Instance.UpdateCardUI(-1);
    }

    #region Bet Actions
    public virtual void SelectRaise()
    {
        Debug.Log(name + " select Raise");

        isActionSelected = true;
        selectedBetAction = BetAction.Raise;
    }
    public virtual void SetRaiseValue(int min)
    {
        int nextBet = StageUIManager.Instance.BetUI.raiseSlider.GetRaiseSliderValue();

        needMoney = nextBet - currentBet;

        currentBet = nextBet;
    }
    public void SelectFold()
    {
        Debug.Log(name + " select Fold");
        isActionSelected = true;
        selectedBetAction = BetAction.Fold;
    }
    public void SelectCall()
    {
        Debug.Log(name + " select Call");
        isActionSelected = true;
        selectedBetAction = BetAction.Call;
    }
    public void SelectAllIn()
    {
        Debug.Log(name + " select All in");
        isActionSelected = true;
        selectedBetAction = BetAction.AllIn;
    }

    public void SelectCheck()
    {
        Debug.Log(name + " select Check");
        isActionSelected = true;
        selectedBetAction = BetAction.Check;
    }

    public virtual IEnumerator WaitForBetCoroutine()
    {
        while (!isActionSelected)
        {
            if (StageUIManager.Instance.BetUI.raiseSlider.IsValueMax())
            {
                StageUIManager.Instance.BetUI.DisableRaiseButton();
            }
            else
            {
                if (availableBetAction.HasFlag(BetAction.Raise))
                {
                    StageUIManager.Instance.BetUI.EnableRaiseButton();
                }
            }
            yield return null;
        }
    }

    public void ResetAction()
    {
        isActionSelected = false;
        selectedBetAction = BetAction.None;
    }

    public void InitBetStates()
    {
        ResetAction();
        currentBet = 0;
        needMoney = 0;
    }
    #endregion

    #region Money
    public void SetMoney(int value)
    {
        money = value;

        if (this is AIPlayer) { StageUIManager.Instance.UpdateAIMoneyUI(money); }

        else { StageUIManager.Instance.UpdatePlayerMoneyUI(money); }
    }
    public void AddMoney(int value)
    {
        money += value;

        if (this is AIPlayer) { StageUIManager.Instance.UpdateAIMoneyUI(money); }

        else { StageUIManager.Instance.UpdatePlayerMoneyUI(money); }
    }
    #endregion

    #region Initialize
    public void InitializePlayer(int initialMoney)
    {
        this.money = initialMoney;
        cardsInHand.Clear();
        handResult = new();
        ResetAction();
    }
    #endregion
}
