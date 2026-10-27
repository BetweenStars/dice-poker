using System.Collections.Generic;
using UnityEngine;

public class ChoiceUIs : MonoBehaviour
{
    public ChoiceButton[] choiceButtons = new ChoiceButton[5];

    public void UpdateChoiceButtons(List<ChoiceOptionData> options)
    {
        foreach (var button in choiceButtons)
        {
            button.Hide();
        }

        if (options == null)
        {
            return;
        }

        for (int i = 0; i < options.Count; i++)
        {
            choiceButtons[i].Show();

            choiceButtons[i].SetChoiceButton(options[i], i);
        }
    }
}