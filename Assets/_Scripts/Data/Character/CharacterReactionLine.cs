using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "New Reaction Line", menuName = "Character/Reaction Line")]
public class CharacterReactionLine : ScriptableObject
{
    //자신의 액션
    public List<string> onCheck = new();
    public List<string> onCall = new();
    public List<string> onFold = new();
    public List<string> onRaise = new();
    public List<string> onAllIn = new();

    //상대 액션 반응
    public List<string> onChecked = new();
    public List<string> onCalled = new();
    public List<string> onFolded = new();
    public List<string> onRaised = new();
    public List<string> onAllIned = new();//이름 이래도 되나?

    public string GetActionLine(BetAction action)
    {
        List<string> lines = action switch
        {
            BetAction.Check => onCheck,
            BetAction.Call => onCall,
            BetAction.Fold => onFold,
            BetAction.Raise => onRaise,
            BetAction.AllIn => onAllIn,
            _ => null
        };

        if (lines == null || lines.Count == 0)
            return string.Empty;

        return lines[UnityEngine.Random.Range(0, lines.Count)];
    }
    public string GetReactionLine(BetAction action)
    {
        List<string> lines = action switch
        {
            BetAction.Check => onChecked,
            BetAction.Call => onCalled,
            BetAction.Fold => onFolded,
            BetAction.Raise => onRaised,
            BetAction.AllIn => onAllIned,
            _ => null
        };

        if (lines == null || lines.Count == 0)
            return string.Empty;

        return lines[UnityEngine.Random.Range(0, lines.Count)];
    }
}
