using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public PlayData Play = new();
    public ProgressData Progress = new();
    public MusicData Music = new();
}
