using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

[Serializable]
public abstract class BaseStageNodeData
{
    public string nodeGuid;
    public string nodeName;
    public Vector2 nodePosition;
    public StageNodeType nodeType;
}

#region dialogue-node
[Serializable]
public class DialogueNodeData : BaseStageNodeData
{
    public CharacterId charId;
    public string text;
    public string nextNodeGuid;
}

[Serializable]
public class ChoiceNodeData : BaseStageNodeData
{
    public List<ChoiceOptionData> choiceOptions;
}

[Serializable]
public class ChoiceOptionData
{
    public string optionText;
    public string nextNodeGuid;
}

[Serializable]
public class SpriteNodeData : BaseStageNodeData
{
    public SpriteActionType spriteAction;
    public CharacterId charId;
    public int position;
    public SpriteSpeedType speed;
    public string spriteId;

    public string nextNodeGuid;
}

public enum SpriteActionType
{
    Show,
    Hide,
    Move,
    Sprite
}

public enum SpriteSpeedType
{
    Slow,
    Normal,
    Fast
}
#endregion

#region audio-camera-node
[Serializable]
public class AudioNodeData : BaseStageNodeData
{
    public StageAudioType audioType;
    //아래 참조는 bgm이면 BGMData를 SFX면 뭐 다른 참조
    public AssetReferenceT<AudioClip> clipRef;
    public string nextNodeGuid;
}
#endregion

#region logic-node
[Serializable]
public class EndNodeData : BaseStageNodeData { }

[Serializable]
public class PassOnlyNodeData : BaseStageNodeData
{
    public string nextNodeGuid;
}

[Serializable]
public class StartNodeData : BaseStageNodeData
{
    public string nextNodeGuid;
}
#endregion