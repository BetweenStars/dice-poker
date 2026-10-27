using System;
using UnityEngine;

public enum CardSuit
{
    Clover = 1,
    Heart = 2,
    Diamond = 3,
    Spade = 4
}

public enum CardRank
{
    None = 0, // 뒷면
    A = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    J = 11,
    Q = 12,
    K = 13
}

[Serializable]
public class CardData
{
    public CardSuit suit;
    public CardRank rank;

    public Sprite sprite;
}
