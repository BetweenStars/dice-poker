using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class CharacterPortraitSprite
{
    public string id;
    public AssetReferenceSprite sprite;
}

[CreateAssetMenu(fileName = "New Character Asset", menuName = "Character/Character Asset")]
public class CharacterAsset : ScriptableObject
{
    public CharacterId id;

    //블러프 폴드 등 행동변수

    //지금은 캐릭터 스프라이트가 분리 안되어 있어서 sprite로 만듬
    //나중에 몸 눈 입으로 구분
    public List<CharacterPortraitSprite> PortraitSprites;
    public AssetReferenceSprite AllInSprite;

    public CharacterReactionLine ReactionLine;
}