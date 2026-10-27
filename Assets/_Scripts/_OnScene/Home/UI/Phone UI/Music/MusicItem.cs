using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MusicItem : MonoBehaviour, IDataSyncable
{
    private MusicScreen _musicScreen;
    private BGMDefinition _bgmData;

    [SerializeField] private Button _musicItemButton;
    [SerializeField] private TMP_Text _musicNameText;
    [SerializeField] private TMP_Text _musicTimeText;
    [SerializeField] private Button _favorButton;

    private bool _isFavor = false;

    private void Awake()
    {
        _musicScreen = GetComponentInParent<MusicScreen>();

        _musicItemButton.onClick.AddListener(OnClickThisItem);
        _favorButton.onClick.AddListener(OnClickFavorButton);
    }

    public void Initialize(BGMDefinition data)
    {
        _bgmData = data;

        Refresh();
    }

    private void OnClickThisItem()
    {
        _musicScreen.PlayBGM(_bgmData);
    }
    private void OnClickFavorButton()
    {
        _isFavor = !_isFavor;

        if (_isFavor)
        {
            //Debug.Log($"set favor {_bgmData.BGMId}");
            DataManager.Instance.Music.AddFavor(_bgmData.BGMId);
        }
        else
        {
            //Debug.Log($"set favor false {_bgmData.BGMId}");
            DataManager.Instance.Music.RemoveFavor(_bgmData.BGMId);
        }

        Refresh();
    }

    public bool IsFavor() => _isFavor;

    public void SyncData()
    {
        _isFavor = DataManager.Instance.Music.IsFavor(_bgmData.BGMId);

        //Debug.Log($"[Music Item] data syncronized favor{_isFavor}");

        Refresh();
    }

    private void Refresh()
    {
        _musicNameText.text = _bgmData.DisplayText;
        _musicTimeText.text = TimeSpan.FromSeconds(_bgmData.LengthSecond).ToString(@"mm\:ss");

        _isFavor = DataManager.Instance.Music.IsFavor(_bgmData.BGMId);

        if (_isFavor) { _favorButton.image.color = Color.red; }
        else { _favorButton.image.color = Color.white; }
    }
}
