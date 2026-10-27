using System;
using UnityEngine;

public enum MapType
{
    Bar,
    Casino
}

[CreateAssetMenu(fileName = "New Stage Data", menuName = "Stage/Stage Data")]
public class StageData : ScriptableObject
{
    public string stageName;
    public int buyIn;
    public int startingMoney;
    public int ante;

    public string sceneName;
}