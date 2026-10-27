using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public enum AudioType
{
    Master = 0,
    BGM = 1,
    SFX = 2,
    Voice = 3
}

public class AudioSlider : MonoBehaviour, IPointerUpHandler, IDataSyncable
{
    private Slider _slider;

    public AudioType AudioType;

    private void Awake()
    {
        _slider = GetComponentInChildren<Slider>();
    }

    private void OnEnable()
    {
        _slider.onValueChanged.AddListener(OnValueChanged);
    }
    private void OnDisable()
    {
        _slider.onValueChanged.RemoveListener(OnValueChanged);
    }

    private void OnValueChanged(float value)
    {
        Debug.Log($"{AudioType} volume Changed : {value}");
        switch (AudioType)
        {
            case AudioType.Master:
                {
                    SettingManager.Instance.SetMasterVolume(value);
                    break;
                }
            case AudioType.BGM:
                {
                    SettingManager.Instance.SetBGMVolume(value);
                    break;
                }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log($"{AudioType} volume saved");
        switch (AudioType)
        {
            case AudioType.Master:
                {
                    DataManager.Instance.SaveMasterVolume();
                    break;
                }
            case AudioType.BGM:
                {
                    DataManager.Instance.SaveBGMVolume();
                    break;
                }
        }
    }

    public void SyncData()
    {
        switch (AudioType)
        {
            case AudioType.Master:
                {
                    _slider.value = DataManager.Instance.Setting.MasterVolume;
                    break;
                }
            case AudioType.BGM:
                {
                    _slider.value = DataManager.Instance.Setting.BGMVolume;
                    break;
                }
        }
    }
}
