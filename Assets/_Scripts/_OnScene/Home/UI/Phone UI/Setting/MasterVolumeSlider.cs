using UnityEngine.EventSystems;

public class MasterVolumeSlider : BaseVolumeSlider
{
    protected override void OnSliderValueChanged(float value)
    {
        SettingManager.Instance.SetMasterVolume(value);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        SettingManager.Instance.SetMasterVolume(_slider.value);
        DataManager.Instance.SaveMasterVolume();
    }

    public override void SyncData()
    {
        SetSliderValue(DataManager.Instance.Setting.MasterVolume);
    }
}