using System.Collections.Generic;

public class MusicData
{
    public string CurrentBGMId = string.Empty;
    public HashSet<string> FavorBGMIds = new();

    public void AddFavor(string bgmId) => FavorBGMIds.Add(bgmId);
    public void RemoveFavor(string bgmId) => FavorBGMIds.Remove(bgmId);
    public bool IsFavor(string bgmId) => FavorBGMIds.Contains(bgmId);
}