using UnityEngine;

public static class TypingSpeedConfigs
{
    public const float SLOW = 0.1f;
    public const float NORMAL = 0.07f;
    public const float FAST = 0.05f;
}

public class SettingManager : MonoBehaviour
{
    public static SettingManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    #region typing-setting
    public void SetTypingSpeed(float speed)
    {
        DataManager.Instance.Setting.TypingSpeed = speed;
    }
    #endregion

    #region volume-setting
    public void SetMasterVolume(float volume)
    {
        DataManager.Instance.Setting.MasterVolume = volume;
        AudioManager.Instance.SetMasterVolume(volume);
    }

    public void SetBGMVolume(float volume)
    {
        DataManager.Instance.Setting.BGMVolume = volume;
        AudioManager.Instance.SetBGMVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        DataManager.Instance.Setting.SFXVolume = volume;
        AudioManager.Instance.SetSFXVolume(volume);
    }

    public void SetVoiceVolume(float volume)
    {
        DataManager.Instance.Setting.VoiceVolume = volume;
        AudioManager.Instance.SetVoiceVolume(volume);
    }
    #endregion

    public void ResetToDefaults()
    {
        DataManager.Instance.Setting = new();

        DataManager.Instance.SaveAllSettings();

        ApplyAllSettings();
    }

    public void ApplyAllSettings()
    {
        AudioManager.Instance.SetMasterVolume(DataManager.Instance.Setting.MasterVolume);
        AudioManager.Instance.SetBGMVolume(DataManager.Instance.Setting.BGMVolume);
        AudioManager.Instance.SetSFXVolume(DataManager.Instance.Setting.SFXVolume);
        AudioManager.Instance.SetVoiceVolume(DataManager.Instance.Setting.VoiceVolume);

        //타이핑 속도는 변수 그 자체를 사용하면 되므로 따로 적용할 필요 없음

        //해상도 적용 만들기
    }
}
