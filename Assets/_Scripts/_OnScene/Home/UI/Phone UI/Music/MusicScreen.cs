using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class MusicScreen : BaseScreen, ILoadingAsyncInitializable
{
    [SerializeField] private MusicItem _musicItemPrefab;
    [SerializeField] private Transform _musicItemContent;

    private List<BGMDefinition> _playlist = new();

    private List<MusicItem> _musicItems = new();

    [SerializeField] private Button _favorOnlyButton;
    [SerializeField] private PlaybackControls _playbackControls;

    public BGMDefinition CurrentBGMData;
    private bool _isFavorOnly = false;
    private bool _isPlaying = false;
    public bool IsPlaying() => _isPlaying;

    #region initialize
    protected override void Awake()
    {
        base.Awake();

        _favorOnlyButton.onClick.AddListener(OnClickFavorOnlyButton);
    }

    public void InitializeOnLoading()
    {
        _playlist = ResourceManager.Instance.GetPlaylist();
        Debug.Log($"playlist count : {_playlist.Count}");

        _isFavorOnly = false;

        _musicItems.Clear();

        foreach (var music in _playlist)
        {
            _musicItems.Add(InstantiateMusicItem(music));
        }

        ShowMusicItems();
    }

    public void OnReadyToStart()
    {
        if (_playlist.Count == 0) return;

        if (string.IsNullOrEmpty(DataManager.Instance.Music.CurrentBGMId))
        {
            PlayBGM(_playlist.Find(music => music.BGMId == "Cozy Home"));
        }
        else
        {
            PlayBGM(_playlist.Find(music => music.BGMId == DataManager.Instance.Music.CurrentBGMId));
        }
    }
    #endregion

    #region favor
    private void OnClickFavorOnlyButton()
    {
        _isFavorOnly = !_isFavorOnly;

        if (_isFavorOnly) { _favorOnlyButton.image.color = Color.red; }
        else { _favorOnlyButton.image.color = Color.white; }

        ShowMusicItems();
    }
    #endregion

    #region music-item
    private void ShowMusicItems()
    {
        foreach (var item in _musicItems)
        {
            item.gameObject.SetActive(false);
        }

        if (!_isFavorOnly)
        {
            foreach (var item in _musicItems)
            {
                item.gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (var item in _musicItems)
            {
                if (item.IsFavor())
                {
                    item.gameObject.SetActive(true);
                }
            }
        }
    }

    private MusicItem InstantiateMusicItem(BGMDefinition data)
    {
        MusicItem item = Instantiate(_musicItemPrefab, _musicItemContent);

        item.Initialize(data);

        return item;
    }
    #endregion

    #region music-playback

    public bool TogglePlayPause()
    {
        _isPlaying = !_isPlaying;

        if (_isPlaying)
        {
            AudioManager.Instance.ResumeBGM();
        }
        else
        {
            AudioManager.Instance.PauseBGM();
        }

        return _isPlaying;
    }
    public void PlayBGM(BGMDefinition bmg)
    {
        CurrentBGMData = bmg;

        AudioManager.Instance.PlayBGMAsync(bmg.clipRef).Forget(exception => Debug.LogError(exception));

        _isPlaying = true;

        _playbackControls.Initialize();
    }
    public void PlayNextMusic()
    {
        int index = _playlist.FindIndex(bgm => bgm.BGMId == CurrentBGMData.BGMId);

        index = (index + 1) % _playlist.Count;

        PlayBGM(_playlist[index]);
    }
    public void PlayPrevMusic()
    {
        int index = _playlist.FindIndex(bgm => bgm.BGMId == CurrentBGMData.BGMId);

        index = (index - 1 + _playlist.Count) % _playlist.Count;

        PlayBGM(_playlist[index]);
    }
    #endregion
}
