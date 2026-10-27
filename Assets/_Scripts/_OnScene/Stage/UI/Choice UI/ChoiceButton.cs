using UnityEngine.UI;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Button))]
public class ChoiceButton : BaseStageUI
{
    private StageNodeController controller;
    private Button button;
    public int my_index;

    protected override void Awake()
    {
        base.Awake();

        controller = FindAnyObjectByType<StageNodeController>();
        button = GetComponent<Button>();
        button.onClick.AddListener(() => controller.OnClickChoiceButton(my_index));
    }

    public void SetChoiceButton(ChoiceOptionData option, int index = -1)
    {
        if (index == -1) return;

        my_index = index;
        button.GetComponentInChildren<TMP_Text>().text = option.optionText;
    }
}