using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class PlayerPrefsKey
{
    public const string TypingSpeed = "TypingSpeed";
    public const string MasterVolume = "MasterVolume";
    public const string BGMVolume = "BGMVolume";
    public const string SFXVolume = "SFXVolume";
    public const string VoiceVolume = "VoiceVolume";
}

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    public event Action OnDataLoaded;

    private void Awake()
    {
        Instance = this;
    }

    #region save-data
    private GameData _data = new();

    public PlayData Play => _data.Play;
    public ProgressData Progress => _data.Progress;
    public MusicData Music => _data.Music;

    public void SaveData(int index)
    {
        DataUtility.Save(index, _data);
    }
    public async UniTask LoadData(int index)
    {
        _data = DataUtility.Load(index);
        OnDataLoaded?.Invoke();

        await ScreenFader.Instance.FadeOutAsync(0.7f);
        await ScreenFader.Instance.FadeInAsync(1.0f);
    }
    #endregion

    #region pref-data
    public SettingData Setting = new();

    public void SaveMasterVolume()
    {
        PlayerPrefs.SetFloat(PlayerPrefsKey.MasterVolume, Setting.MasterVolume);
        PlayerPrefs.Save();
    }

    public void SaveBGMVolume()
    {
        PlayerPrefs.SetFloat(PlayerPrefsKey.BGMVolume, Setting.BGMVolume);
        PlayerPrefs.Save();
    }

    public void SaveSFXVolume()
    {
        PlayerPrefs.SetFloat(PlayerPrefsKey.SFXVolume, Setting.SFXVolume);
        PlayerPrefs.Save();
    }

    public void SaveVoiceVolume()
    {
        PlayerPrefs.SetFloat(PlayerPrefsKey.VoiceVolume, Setting.VoiceVolume);
        PlayerPrefs.Save();
    }

    public void SaveTypingSpeed()
    {
        PlayerPrefs.SetFloat(PlayerPrefsKey.TypingSpeed, Setting.TypingSpeed);
        PlayerPrefs.Save();
    }

    public void LoadAllSettings()
    {
        Setting.MasterVolume = PlayerPrefs.GetFloat(PlayerPrefsKey.MasterVolume, 1f);
        Setting.BGMVolume = PlayerPrefs.GetFloat(PlayerPrefsKey.BGMVolume, 1f);
        Setting.SFXVolume = PlayerPrefs.GetFloat(PlayerPrefsKey.SFXVolume, 1f);
        Setting.VoiceVolume = PlayerPrefs.GetFloat(PlayerPrefsKey.VoiceVolume, 1f);

        Setting.TypingSpeed = PlayerPrefs.GetFloat(PlayerPrefsKey.TypingSpeed, TypingSpeedConfigs.NORMAL);

        SettingManager.Instance.ApplyAllSettings();
    }

    public void SaveAllSettings()
    {
        PlayerPrefs.SetFloat(PlayerPrefsKey.MasterVolume, Setting.MasterVolume);
        PlayerPrefs.SetFloat(PlayerPrefsKey.BGMVolume, Setting.BGMVolume);
        PlayerPrefs.SetFloat(PlayerPrefsKey.SFXVolume, Setting.SFXVolume);
        PlayerPrefs.SetFloat(PlayerPrefsKey.VoiceVolume, Setting.VoiceVolume);

        PlayerPrefs.SetFloat(PlayerPrefsKey.TypingSpeed, Setting.TypingSpeed);

        PlayerPrefs.Save();
    }
    #endregion
}
