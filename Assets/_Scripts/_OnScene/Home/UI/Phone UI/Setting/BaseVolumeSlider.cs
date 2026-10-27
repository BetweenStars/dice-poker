using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class BaseVolumeSlider : MonoBehaviour, IPointerUpHandler, IDataSyncable
{
    [SerializeField] protected Slider _slider;

    protected virtual void Awake()
    {
        if (_slider == null) _slider = GetComponentInChildren<Slider>();

        _slider.onValueChanged.AddListener(OnSliderValueChanged);
    }
    
    protected void SetSliderValue(float value) => _slider.value = value;

    protected abstract void OnSliderValueChanged(float value);
    public abstract void OnPointerUp(PointerEventData eventData);
    public abstract void SyncData();
}