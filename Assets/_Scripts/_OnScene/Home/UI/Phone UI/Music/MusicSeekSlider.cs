using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MusicSeekSlider : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Slider slider;
    private bool _isDragging = false;
    private void Update()
    {
        if (!_isDragging)
        {
            slider.value = AudioManager.Instance.GetBGMSeek();
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        AudioManager.Instance.SeekBGM(slider.value);
        
        _isDragging = false;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        _isDragging = true;
    }
}