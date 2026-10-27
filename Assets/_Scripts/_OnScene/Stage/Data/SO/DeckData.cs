using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Deck Data", menuName = "Deck/Deck")]
public class DeckData : ScriptableObject
{
    public List<CardData> cardDatas;

    public Sprite cardBackSprite;
}