using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private const float MinVolumeDb = -80f;
    private const string MasterVolumeParameter = "MasterVolume";
    private const string BGMVolumeParameter = "BGMVolume";
    private const string SFXVolumeParameter = "SFXVolume";
    private const string VoiceVolumeParameter = "VoiceVolume";

    [Header("Audio Source")]
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioSource _voiceSource;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer _audioMixer;

    private void Awake()
    {
        Instance = this;
    }

    #region bgm
    public async UniTask PlayBGMAsync(AssetReferenceT<AudioClip> clipRef)
    {
        AudioClip newClip = await ResourceManager.Instance.LoadBGMClip(clipRef);

        if (newClip == null) { return; }

        if (_bgmSource.isPlaying && newClip == _bgmSource.clip) { return; }

        _bgmSource.DOKill();

        _bgmSource.clip = newClip;
        _bgmSource.volume = 1f;

        _bgmSource.Play();
    }
    public void PauseBGM()
    {
        if (_bgmSource.clip == null) { return; }

        _bgmSource.Pause();
    }
    public void ResumeBGM()
    {
        if (_bgmSource.clip == null) { return; }

        _bgmSource.UnPause();
    }
    public void SeekBGM(float normalizedTime)
    {
        AudioClip clip = _bgmSource.clip;

        if (clip == null || clip.samples <= 0) { return; }

        float clampedTime = Mathf.Clamp01(normalizedTime);
        int sample = Mathf.Clamp(Mathf.RoundToInt((clip.samples - 1) * clampedTime), 0, clip.samples - 1);

        _bgmSource.timeSamples = sample;
    }
    public void StopBGM()
    {
        _bgmSource.Stop();
        _bgmSource.clip = null;
    }
    public float GetBGMSeek()
    {
        AudioClip clip = _bgmSource.clip;

        if (clip == null || clip.samples <= 0) { return 0f; }

        return Mathf.Clamp01((float)_bgmSource.timeSamples / (clip.samples - 1));
    }
    public float GetBGMTotalTime()
    {
        AudioClip clip = _bgmSource.clip;

        if (clip == null || clip.samples <= 0) { return 0f; }

        return clip.length;
    }
    public float GetBGMCurrentTime()
    {
        AudioClip clip = _bgmSource.clip;

        if (clip == null || clip.samples <= 0) { return 0f; }

        return _bgmSource.time;
    }
    #endregion

    #region sfx
    public void PlaySFX(string key)
    {
        _sfxSource.PlayOneShot(ResourceManager.Instance.GetSFXClip(key));
    }
    #endregion

    #region volume
    public void SetMasterVolume(float volume) { SetMixerVolume(MasterVolumeParameter, volume); }
    public void SetBGMVolume(float volume) { SetMixerVolume(BGMVolumeParameter, volume); }
    public void SetSFXVolume(float volume) { SetMixerVolume(SFXVolumeParameter, volume); }
    public void SetVoiceVolume(float volume) { SetMixerVolume(VoiceVolumeParameter, volume); }

    private void SetMixerVolume(string parameterName, float volume)
    {
        float volumeDb = volume <= 0f ? MinVolumeDb : Mathf.Log10(volume) * 20f;
        _audioMixer.SetFloat(parameterName, volumeDb);
    }
    #endregion
}
