using UnityEngine.EventSystems;

public class BGMVolumeSlider : BaseVolumeSlider
{
    protected override void OnSliderValueChanged(float value)
    {
        SettingManager.Instance.SetBGMVolume(value);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        SettingManager.Instance.SetBGMVolume(_slider.value);
        DataManager.Instance.SaveBGMVolume();
    }

    public override void SyncData()
    {
        SetSliderValue(DataManager.Instance.Setting.BGMVolume);
    }
}