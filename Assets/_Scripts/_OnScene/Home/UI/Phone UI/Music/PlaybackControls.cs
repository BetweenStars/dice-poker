using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlaybackControls : MonoBehaviour
{
    private MusicScreen _musicScreen;

    [SerializeField] private Button _playButton;
    [SerializeField] private Button _prevButton;
    [SerializeField] private Button _nextButton;

    [SerializeField] private TMP_Text _currentMusicTitle;
    [SerializeField] private TMP_Text _currentTimeText;
    [SerializeField] private TMP_Text _totalTimeText;

    private void Awake()
    {
        _musicScreen = GetComponentInParent<MusicScreen>();

        _playButton.onClick.AddListener(OnClickPlay);
        _prevButton.onClick.AddListener(OnClickPrev);
        _nextButton.onClick.AddListener(OnClickNext);
    }

    public void Update()
    {
        _currentTimeText.text = TimeSpan.FromSeconds(AudioManager.Instance.GetBGMCurrentTime()).ToString(@"mm\:ss");
    }

    public void Initialize()
    {
        bool isPlaying = _musicScreen.IsPlaying();

        if (isPlaying)
        {
            _playButton.image.color = Color.white;
        }
        else
        {
            _playButton.image.color = Color.gray7;
        }

        _currentMusicTitle.text = _musicScreen.CurrentBGMData.DisplayText;
        _totalTimeText.text = TimeSpan.FromSeconds(_musicScreen.CurrentBGMData.LengthSecond).ToString(@"mm\:ss");
    }

    private void OnClickPlay()
    {
        bool isPlaying = _musicScreen.TogglePlayPause();

        if (isPlaying)
        {
            _playButton.image.color = Color.white;
        }
        else
        {
            _playButton.image.color = Color.gray7;
        }
    }
    private void OnClickPrev() => _musicScreen.PlayPrevMusic();
    private void OnClickNext() => _musicScreen.PlayNextMusic();
}
