using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "New BGM Def", menuName = "BGM/BGM Def")]
public class BGMDefinition : ScriptableObject
{
    public string BGMId;
    public string DisplayText;
    public int LengthSecond = 0;
    public AssetReferenceT<AudioClip> clipRef;
}