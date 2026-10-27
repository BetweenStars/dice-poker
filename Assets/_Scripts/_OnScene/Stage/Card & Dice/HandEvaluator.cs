using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

public enum HandRank
{
    None,
    HighCard,
    OnePair,
    TwoPair,
    ThreeOfAKind,
    Flush,
    SmallStraight,
    FullHouse,
    LargeStraight,
    FourOfAKind,
    FiveOfAKind
}

public class HandResult
{
    public HandRank handRank = HandRank.None;

    public int mainValue = 0;
    public int subValue = 0;
    public int highCard = 0;
    public bool isUsing4Cards = false;

    public List<int> remainNums = new();

    public void AdjustForCompare()
    {
        if (mainValue == 1)
        {
            if (handRank != HandRank.SmallStraight && handRank != HandRank.LargeStraight)
            {
                mainValue = 14;
            }
        }
        if (subValue == 1) { subValue = 14; }
        if (highCard == 1) { highCard = 14; }

        if (remainNums != null && remainNums.Count > 0)
        {
            for (int i = 0; i < remainNums.Count; i++)
            {
                if (remainNums[i] == 1)
                {
                    remainNums[i] = 14;
                }
            }

            remainNums.Sort();
            remainNums.Reverse();
        }
    }
}

public static class HandEvaluator
{
    public static HandResult Evaluate(List<CardData> handCards, int dice1, int dice2)
    {
        HandResult result = new();

        int[] numberCounts_5 = new int[14];
        int[] numberCounts_4 = new int[14];

        List<int> numberList_5 = new();
        List<int> numberList_4 = new();

        foreach (var card in handCards)
        {
            numberCounts_5[(int)card.rank]++;
            numberCounts_4[(int)card.rank]++;

            numberList_5.Add((int)card.rank);
            numberList_4.Add((int)card.rank);
        }

        numberCounts_5[dice1]++;
        numberCounts_5[dice2]++;
        numberList_5.Add(dice1);
        numberList_5.Add(dice2);

        numberCounts_4[dice1 + dice2]++;
        numberList_4.Add(dice1 + dice2);

        numberList_5.Sort();
        numberList_4.Sort();

        #region Five-of-Kind
        for (int i = 13; i > 0; i--)
        {
            if (numberCounts_5[i] == 5)
            {
                result.handRank = HandRank.FiveOfAKind;
                result.mainValue = i;
                return result;
            }
        }
        #endregion

        #region Four-of-Kind
        for (int i = 13; i > 0; i--)
        {
            if (numberCounts_5[i] == 4)
            {
                result.handRank = HandRank.FourOfAKind;
                result.mainValue = i;
                for (int j = 0; j < numberList_5.Count; j++)
                {
                    if (numberList_5[j] != i) { result.subValue = numberList_5[j]; }
                }
                return result;
            }

            if (numberCounts_4[i] == 4)
            {
                result.handRank = HandRank.FourOfAKind;
                result.mainValue = i;
                result.isUsing4Cards = true;
                return result;
            }
        }
        #endregion

        #region Large-Straight
        HashSet<int> numberHash_5 = new(numberList_5);

        if (numberHash_5.Count == 5)
        {
            int lowest = numberList_5[0];
            bool isLargeStraight = true;

            for (int i = 1; i < numberList_5.Count; i++)
            {
                if (lowest + i != numberList_5[i])
                {
                    isLargeStraight = false;
                    break;
                }
            }

            if (isLargeStraight)
            {
                result.handRank = HandRank.LargeStraight;
                result.mainValue = lowest;
                return result;
            }
        }
        #endregion 

        #region Full-House
        bool hasTripleAt5 = false;
        bool hasTripleAt4 = false;
        int threeMain_5 = 0;
        int threeMain_4 = 0;

        //트리플 체크 파트
        for (int i = 13; i > 0; i--)
        {
            if (numberCounts_5[i] == 3)
            {
                hasTripleAt5 = true;
                threeMain_5 = i;
            }
            if (numberCounts_4[i] == 3)
            {
                hasTripleAt4 = true;
                threeMain_4 = i;
            }
        }

        if (hasTripleAt5)
        {
            for (int i = 13; i > 0; i--)
            {
                if (numberCounts_5[i] == 2)
                {
                    result.handRank = HandRank.FullHouse;
                    result.mainValue = threeMain_5;
                    result.subValue = i;
                    return result;
                }
            }
        }
        #endregion

        #region Small-Straight
        HashSet<int> numberHash_4 = new(numberList_4);
        bool isSmallStraight = false;

        List<int> uniqueList = new(numberHash_5);
        uniqueList.Sort();

        if (uniqueList.Count == 5)
        {
            List<int> list1 = new();
            List<int> list2 = new();
            list1.AddRange(uniqueList.GetRange(1, 4));
            list2.AddRange(uniqueList.GetRange(0, 4));
            list1.Sort();
            list2.Sort();

            int lowest1 = list1[0];
            isSmallStraight = true;

            for (int i = 1; i < list1.Count; i++)
            {
                if (lowest1 + i != list1[i])
                {
                    isSmallStraight = false;
                    break;
                }
            }

            if (!isSmallStraight)
            {
                int lowest2 = list2[0];
                isSmallStraight = true;

                for (int i = 1; i < list2.Count; i++)
                {
                    if (lowest2 + i != list2[i])
                    {
                        isSmallStraight = false;
                        break;
                    }
                }

                if (isSmallStraight)
                {
                    result.mainValue = lowest2;
                    result.remainNums.Add(uniqueList[uniqueList.Count - 1]);
                }
            }
            else
            {
                result.mainValue = lowest1;
                result.remainNums.Add(uniqueList[0]);
            }
        }
        else if (uniqueList.Count == 4)
        {
            List<int> list = new();
            list.AddRange(uniqueList);
            list.Sort();

            int lowest = list[0];
            isSmallStraight = true;

            for (int i = 1; i < list.Count; i++)
            {
                if (lowest + i != list[i])
                {
                    isSmallStraight = false;
                    break;
                }
            }

            if (isSmallStraight)
            {
                result.mainValue = lowest;
                for (int i = 1; i <= 13; i++)
                {
                    if (numberCounts_5[i] == 2)
                    {
                        result.remainNums.Add(i);
                    }
                }
            }
        }

        if (isSmallStraight)
        {
            result.isUsing4Cards = false;
            result.handRank = HandRank.SmallStraight;
        }

        if (numberHash_4.Count == 4)
        {
            int lowest = numberList_4[0];


            isSmallStraight = true;

            for (int i = 1; i < numberList_4.Count; i++)
            {
                if (lowest + i != numberList_4[i])
                {
                    isSmallStraight = false;
                    break;
                }
            }

            if (isSmallStraight)
            {
                if (lowest > result.mainValue)
                {
                    result.mainValue = lowest;
                    result.remainNums.Clear();
                    result.isUsing4Cards = true;
                }
                result.handRank = HandRank.SmallStraight;
            }
        }

        if (isSmallStraight)
        {
            return result;
        }
        #endregion

        #region Three-of-Kind
        if (hasTripleAt5)
        {
            for (int i = numberList_5.Count - 1; i >= 0; i--)
            {
                if (numberList_5[i] != threeMain_5)
                {
                    result.remainNums.Add(numberList_5[i]);
                }
            }
            result.mainValue = threeMain_5;
            result.isUsing4Cards = false;
            result.handRank = HandRank.ThreeOfAKind;
            return result;
        }
        if (hasTripleAt4)
        {
            for (int i = numberList_4.Count - 1; i >= 0; i--)
            {
                if (numberList_4[i] != threeMain_4)
                {
                    result.remainNums.Add(numberList_4[i]);
                }
            }
            result.mainValue = threeMain_4;
            result.isUsing4Cards = true;
            result.handRank = HandRank.ThreeOfAKind;
            return result;
        }
        #endregion

        #region Two-Pair
        bool isTwoPair_5 = false;
        int pairCount = 0;
        int main = 0;
        int sub = 0;
        int remain_5 = 0;

        for (int i = 13; i > 0; i--)
        {
            if (numberCounts_5[i] == 2)
            {
                pairCount++;
                if (pairCount == 1) { main = i; }
                if (pairCount == 2)
                {
                    sub = i;
                    for (int j = 0; j < numberList_5.Count; j++)
                    {
                        if (numberList_5[j] != main && numberList_5[j] != sub)
                        {
                            remain_5 = numberList_5[j];
                        }
                    }
                    result.isUsing4Cards = false;
                    isTwoPair_5 = true;
                }
            }
        }

        bool isTwoPair_4 = false;
        int main2 = 0;
        int sub2 = 0;

        pairCount = 0;

        for (int i = 13; i > 0; i--)
        {
            if (numberCounts_4[i] == 2)
            {
                pairCount++;
                if (pairCount == 1) { main2 = i; }
                if (pairCount == 2)
                {
                    sub2 = i;
                    isTwoPair_4 = true;
                }
            }
        }

        if (isTwoPair_4 || isTwoPair_5)
        {
            result.handRank = HandRank.TwoPair;

            if (main > main2)
            {
                result.mainValue = main;
                result.subValue = sub;
                result.remainNums.Add(remain_5);
                result.isUsing4Cards = false;
            }
            else if (main < main2)
            {
                result.isUsing4Cards = true;
                result.mainValue = main2;
                result.subValue = sub2;
            }
            else
            {
                if (sub >= sub2)
                {
                    result.mainValue = main;
                    result.subValue = sub;
                    for (int i = 0; i < numberList_5.Count; i++)
                    {
                        if (numberList_5[i] != main && numberList_5[i] != sub)
                        {
                            result.remainNums.Add(numberList_5[i]);
                        }
                    }
                    result.isUsing4Cards = false;
                }
                else
                {
                    result.mainValue = main2;
                    result.subValue = sub2;
                    result.isUsing4Cards = true;
                }
            }

            return result;
        }
        #endregion

        #region One-Pair
        bool isPair = false;
        int pairNum = 0;

        for (int i = 13; i > 0; i--)
        {
            if (numberCounts_5[i] == 2)
            {
                pairNum = i;
                isPair = true;
                result.isUsing4Cards = false;
                for (int j = numberList_5.Count - 1; j >= 0; j--)
                {
                    if (numberList_5[j] != pairNum)
                    {
                        result.remainNums.Add(numberList_5[j]);
                    }
                }
            }
        }
        for (int i = 13; i > 0; i--)
        {
            if (numberCounts_4[i] == 2)
            {
                if (i <= pairNum) { break; }
                else
                {
                    pairNum = i;
                    isPair = true;
                    result.isUsing4Cards = true;
                    result.remainNums.Clear();
                    for (int j = numberList_4.Count - 1; j >= 0; j--)
                    {
                        if (numberList_4[j] != pairNum)
                        {
                            result.remainNums.Add(numberList_4[j]);
                        }
                    }
                }
            }
        }
        if (isPair)
        {
            result.handRank = HandRank.OnePair;
            result.mainValue = pairNum;
            return result;
        }
        #endregion

        #region High-Card
        result.handRank = HandRank.HighCard;
        result.remainNums = new(numberList_5);
        result.remainNums.Reverse();
        return result;
        #endregion
    }

    public static Player CompareHands(Player player1, Player player2)
    {
        HandResult result1 = player1.handResult;
        HandResult result2 = player2.handResult;

        result1.AdjustForCompare();
        result2.AdjustForCompare();

        if (result1.handRank > result2.handRank)
            return player1;
        else if (result1.handRank < result2.handRank)
            return player2;
        else
        {
            HandRank rank = result1.handRank;

            HandResult main = CompareMainValue(result1, result2);
            HandResult sub = CompareSubValue(result1, result2);
            HandResult remain = CompareRemainNums(result1, result2);

            if (rank == HandRank.FiveOfAKind || rank == HandRank.LargeStraight)
            {
                return ResultToPlayer(main);
            }
            if (rank == HandRank.FourOfAKind)
            {
                if (main == null)
                {
                    if (result1.isUsing4Cards && !result2.isUsing4Cards) { return player2; }
                    else if (!result1.isUsing4Cards && result2.isUsing4Cards) { return player1; }
                    else { return ResultToPlayer(sub); }
                }
                else
                {
                    return ResultToPlayer(main);
                }
            }
            if (rank == HandRank.FullHouse)
            {
                if (main == null)
                {
                    if (sub == null) { return null; }
                    else { return ResultToPlayer(sub); }
                }
                else
                {
                    return ResultToPlayer(main);
                }
            }
            if (rank == HandRank.SmallStraight)
            {
                if (main == null)
                {
                    if (result1.isUsing4Cards && !result2.isUsing4Cards) { return player2; }
                    else if (!result1.isUsing4Cards && result2.isUsing4Cards) { return player1; }
                    else { return ResultToPlayer(remain); }
                }
                else
                {
                    return ResultToPlayer(main);
                }
            }
            if (rank == HandRank.ThreeOfAKind)
            {
                if (main == null)
                {
                    if (result1.isUsing4Cards && !result2.isUsing4Cards) { return player2; }
                    else if (!result1.isUsing4Cards && result2.isUsing4Cards) { return player1; }
                    else { return ResultToPlayer(remain); }
                }
                else
                {
                    return ResultToPlayer(main);
                }
            }
            if (rank == HandRank.TwoPair)
            {
                if (main == null)
                {
                    if (sub == null)
                    {
                        if (result1.isUsing4Cards && !result2.isUsing4Cards) { return player2; }
                        else if (!result1.isUsing4Cards && result2.isUsing4Cards) { return player1; }
                        else { return ResultToPlayer(remain); }
                    }
                    else
                    {
                        return ResultToPlayer(sub);
                    }
                }
                else
                {
                    return ResultToPlayer(main);
                }
            }
            if (rank == HandRank.OnePair)
            {
                if (main == null)
                {
                    if (result1.isUsing4Cards && !result2.isUsing4Cards) { return player2; }
                    else if (!result1.isUsing4Cards && result2.isUsing4Cards) { return player1; }
                    else { return ResultToPlayer(remain); }
                }
                else
                {
                    return ResultToPlayer(main);
                }
            }
            if (rank == HandRank.HighCard)
            {
                if (main != null) { return ResultToPlayer(main); }
                else { return ResultToPlayer(remain); }
            }
        }

        return null;

        Player ResultToPlayer(HandResult result)
        {
            if (result == result1) return player1;
            else if (result == result2) return player2;

            return null;
        }
    }

    private static HandResult CompareMainValue(HandResult r1, HandResult r2)
    {
        if (r1.mainValue > r2.mainValue) return r1;
        else if (r1.mainValue < r2.mainValue) return r2;
        else return null;
    }
    private static HandResult CompareSubValue(HandResult r1, HandResult r2)
    {
        if (r1.subValue > r2.subValue) return r1;
        else if (r1.subValue < r2.subValue) return r2;
        else return null;
    }
    private static HandResult CompareRemainNums(HandResult r1, HandResult r2)
    {
        if (r1.remainNums.Count != r2.remainNums.Count)
        {
            //Debug.LogWarning("Why p1 and p2 have different remainNum Count?");
            return null;
        }

        if (r1.remainNums.Count == 0 || r2.remainNums.Count == 0)
        {
            //Debug.LogWarning("No remain nums in p1 or p2");
            return null;
        }

        for (int i = 0; i < r1.remainNums.Count; i++)
        {
            if (r1.remainNums[i] != r2.remainNums[i])
            {
                return r1.remainNums[i] > r2.remainNums[i] ? r1 : r2;
            }
        }

        //Debug.Log("p1 p2 have same remainnums. Draw...");
        return null;
    }

    public static float GetHandWinRate(List<CardData> handCards, DeckData deck, int? dice1, int? dice2, int simulateCount = 10000)
    {
        int wins = 0;
        int ties = 0;

        List<CardData> simulHandCards = new();

        for (int i = 0; i < simulateCount; i++)
        {
            simulHandCards.Clear();
            simulHandCards.AddRange(handCards);

            List<CardData> simulatedDeck = new List<CardData>(deck.cardDatas);
            foreach (var card in handCards)
            {
                simulatedDeck.RemoveAll(c => c.rank == card.rank && c.suit == card.suit);
            }

            if (handCards.Count < 3)
            {
                // 플레이어의 패 3장 랜덤으로 뽑기
                for (int j = handCards.Count; j < 3; j++)
                {
                    int index = Random.Range(0, simulatedDeck.Count);
                    simulHandCards.Add(simulatedDeck[index]);
                    simulatedDeck.RemoveAt(index);
                }
            }

            // 상대방의 패 3장 랜덤으로 뽑기
            List<CardData> opponentHand = new List<CardData>();
            for (int j = 0; j < 3; j++)
            {
                int index = Random.Range(0, simulatedDeck.Count);
                opponentHand.Add(simulatedDeck[index]);
                simulatedDeck.RemoveAt(index);
            }

            // 주사위 값 랜덤으로 뽑기
            int simulatedDice1 = dice1 ?? Random.Range(1, 7);
            int simulatedDice2 = dice2 ?? Random.Range(1, 7);

            HandResult playerResult = Evaluate(simulHandCards, simulatedDice1, simulatedDice2);
            HandResult opponentResult = Evaluate(opponentHand, simulatedDice1, simulatedDice2);

            if (playerResult.handRank > opponentResult.handRank)
                wins++;
            else if (playerResult.handRank == opponentResult.handRank)
                ties++;
        }
        return (wins + ties * 0.5f) / simulateCount;
    }
}
