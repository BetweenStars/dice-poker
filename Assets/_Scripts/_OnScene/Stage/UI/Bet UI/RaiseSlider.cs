using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RaiseSlider : BaseStageUI
{
    private Slider slider;
    private TMP_Text text;

    protected override void Awake()
    {
        base.Awake();

        slider = GetComponentInChildren<Slider>();
        text = GetComponentInChildren<TMP_Text>();
    }

    public override void Hide()
    {
        base.Hide();
        canvasGroup.alpha = 0.5f;
    }

    public void UpdateRaiseSlider(int minRaise, int maxRaise)
    {
        slider.minValue = minRaise;
        slider.maxValue = maxRaise;

        slider.value = slider.minValue;

        OnRaiseSliderValueChanged();
    }

    public void OnRaiseSliderValueChanged()
    {
        if (GetComponentInChildren<CanvasGroup>().alpha < 1) return;

        int value = (int)slider.value;

        if (value > slider.maxValue - BettingConfigs.RAISESTEP / 2)
        {
            value = (int)slider.maxValue;
        }
        else
        {
            value = Mathf.RoundToInt(

                value / (float)BettingConfigs.RAISESTEP

            ) * BettingConfigs.RAISESTEP;
        }

        slider.value = value;
        text.text = ((int)slider.value).ToString();
    }

    public void SetRaiseSliderValue(int value)
    {
        slider.value = value;
    }
    public int GetRaiseSliderValue()
    {
        return (int)slider.value;
    }
    public bool IsValueMax()
    {
        return slider.value == slider.maxValue;
    }
    public void InitRaiseSlider()
    {
        slider.value = slider.minValue;
    }
}