using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem;
using UnityEngine.AddressableAssets;

public enum EpisodeType
{
    Main,
    Side
}

[Serializable]
public class EpisodeKey
{
    public string RouteId;
    public int Chapter;
    public string EpisodeId;

    public override string ToString() => $"{RouteId}_Ch{Chapter}_{EpisodeId}";
    public bool IsSame(EpisodeKey other)
    {
        if (other == null) return false;

        return RouteId == other.RouteId
            && Chapter == other.Chapter
            && EpisodeId == other.EpisodeId;
    }
}

[CreateAssetMenu(fileName = "New Episode Asset", menuName = "Episode/Episode Asset")]
public class EpisodeAsset : ScriptableObject
{
    //일단 유니티에서 직접입력, 나중에 그래프뷰 블랙보드에 추가예정
    public EpisodeKey EpisodeKey;
    public string Key => EpisodeKey.ToString();

    [Header("Episode Map Info")]
    public MapType Map;
    public EpisodeType EpisodeType;
    public float Weight; // 0 ~ 100 사이드 스토리 등장 확률
    public AssetReferenceSprite backgroundSprite;

    [Header("Node Datas")]
    public StartNodeData startNodeData = new();
    public List<DialogueNodeData> dialogueNodes = new();
    public List<ChoiceNodeData> choiceNodes = new();
    public List<SpriteNodeData> spriteNodes = new();
    public List<AudioNodeData> audioNodes = new();

    public List<PassOnlyNodeData> passOnlyNodes = new();
    public List<EndNodeData> endNodes = new();

    public List<string> addFlags = new();
    public List<string> removeFlages = new();
    public List<string> requiredFlags = new();
    public List<string> blockedFlags = new();
}