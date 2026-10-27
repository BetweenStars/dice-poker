using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

[Serializable]
public class ProgressData
{
    #region story
    public EpisodeKey EpisodeKey = new();
    public string Key => EpisodeKey.ToString();

    public StageData StageData;
    #endregion

    #region flag
    public HashSet<string> ActiveFlag = new();
    public HashSet<string> CompleteEvent = new();
    public Dictionary<CharacterId, int> MeetCount = new();

    public void AddFlag(string flag) { }
    public void RemoveFlag(string flag) { }
    public bool HasFlag(string flag) => ActiveFlag.Contains(flag);
    #endregion
}