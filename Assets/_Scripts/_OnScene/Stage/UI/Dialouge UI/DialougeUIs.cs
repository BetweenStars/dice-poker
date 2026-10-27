using System.Collections.Generic;
using UnityEngine;

public class DialougeUIs : MonoBehaviour
{
    public DialogueBox[] dialogueBoxes = new DialogueBox[5];

    public void HideAllDialgueBox()
    {
        foreach (var box in dialogueBoxes)
        {
            box.Hide();
        }
    }
}
