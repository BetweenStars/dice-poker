using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class TableController : MonoBehaviour
{
    #region settings
    public Transform[] aiCardPositions = new Transform[3];
    public Transform[] myCardPositions = new Transform[3];

    public Transform[] diceWaitTransforms = new Transform[2];
    //0이 플레이어 1이 ai 자리
    public Transform[] diceThrowTransforms = new Transform[2];
    #endregion

    #region  objects
    [SerializeField] private Dice firstDice;
    [SerializeField] private Dice secondDice;

    public Deck deck;
    #endregion

    #region data
    [SerializeField] private DeckData deckData;
    private List<CardData> cardsInDeck = new();

    private Vector3[] waitPos = new Vector3[2];
    private Vector3[] throwPos = new Vector3[2];
    #endregion

    #region dice-funtion
    public IEnumerator MoveDiceToThrowPosition(int diceIndex, int playerIndex)
    {
        Sequence seq = DOTween.Sequence();

        if (diceIndex == 0)
        {
            seq.Join(firstDice.Rotate(diceThrowTransforms[playerIndex].rotation.eulerAngles, 0.5f));
            seq.Join(firstDice.Move(throwPos[playerIndex], 0.5f));
            yield return seq.WaitForCompletion();
        }
        else if (diceIndex == 1)
        {
            seq.Join(secondDice.Rotate(diceThrowTransforms[playerIndex].rotation.eulerAngles, 0.5f));
            seq.Join(secondDice.Move(throwPos[playerIndex], 0.5f));
            yield return seq.WaitForCompletion();
        }
    }
    public IEnumerator RollDice(int index)
    {
        if (index == 0)
        {
            firstDice.Roll();
            yield return firstDice.WaitForStop();
        }
        else if (index == 1)
        {
            secondDice.Roll();
            yield return secondDice.WaitForStop();
        }
    }
    public int GetDiceValue(int index)
    {
        if (index == 0) { return firstDice.GetDiceValue(); }
        else if (index == 1) { return secondDice.GetDiceValue(); }
        return -1;
    }
    public void FixDices()
    {
        firstDice.Fix();
        secondDice.Fix();
    }
    #endregion

    #region card-function
    public IEnumerator ShuffleDeck()
    {
        for (int i = cardsInDeck.Count - 1; i > 0; i--)
        {
            int rand = UnityEngine.Random.Range(0, i + 1);

            (cardsInDeck[i], cardsInDeck[rand]) = (cardsInDeck[rand], cardsInDeck[i]);
        }

        //나중에 shuffle에 합쳐서 펼치고 모으고 섞는 애니메이션 재생
        yield return deck.Shuffle();
        yield return deck.Gather(0.5f);
    }

    public DeckData GetDeckData() => deckData;

    public int drawCount = 0;
    public IEnumerator DrawCard(Player player)
    {
        if (cardsInDeck.Count == 0) { Debug.LogError("Deck is empty!"); yield break; }

        CardData drawnCard = cardsInDeck[^1];
        cardsInDeck.RemoveAt(cardsInDeck.Count - 1);

        AudioManager.Instance.PlaySFX("sfx_taking_card");

        if (player is AIPlayer)
        {
            yield return deck.MoveCardTo(drawCount, aiCardPositions[player.cardsInHand.Count]);
        }
        else
        {
            yield return deck.MoveCardTo(drawCount, myCardPositions[player.cardsInHand.Count]);
        }

        player.AddCard(drawnCard);

        drawCount++;
    }
    #endregion

    #region initialize
    public void Initialize()
    {
        //init transforms to vectors
        for (int i = 0; i < 2; i++)
        {
            waitPos[i] = diceWaitTransforms[i].position;
            throwPos[i] = diceThrowTransforms[i].position;
        }

        //init dice
        firstDice.Move(waitPos[0]);
        secondDice.Move(waitPos[1]);
        firstDice.Rotate(Vector3.zero);
        secondDice.Rotate(Vector3.zero);

        firstDice.SetValue(-1);
        secondDice.SetValue(-1);

        firstDice.SetKinematic(true);
        secondDice.SetKinematic(true);

        //init deck
        cardsInDeck = new();

        foreach (CardData cardData in deckData.cardDatas)
        {
            cardsInDeck.Add(cardData);
        }

        StartCoroutine(deck.Gather(0));

        drawCount = 0;
    }
    public void Reset()
    {
        //주사위 값 리셋
        firstDice.SetValue(-1);
        secondDice.SetValue(-1);

        firstDice.SetKinematic(true);
        secondDice.SetKinematic(true);

        //카드 값 리셋
        cardsInDeck = new();

        foreach (CardData cardData in deckData.cardDatas)
        {
            cardsInDeck.Add(cardData);
        }

        drawCount = 0;
    }

    public IEnumerator ClearTable()
    {
        //카드 모으기
        yield return deck.Gather(0.7f);

        //주사위 모으기
        Sequence firstDiceSeq = DOTween.Sequence();
        firstDiceSeq.Join(firstDice.Rotate(Vector3.zero, 0.5f));
        firstDiceSeq.Join(firstDice.Move(waitPos[0], 0.5f));
        yield return firstDiceSeq.WaitForCompletion();

        Sequence secondDiceSeq = DOTween.Sequence();
        secondDiceSeq.Join(secondDice.Rotate(Vector3.zero, 0.5f));
        secondDiceSeq.Join(secondDice.Move(waitPos[1], 0.5f));
        yield return secondDiceSeq.WaitForCompletion();
    }
    #endregion
}
