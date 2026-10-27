using System.Collections;
using TMPro;
using UnityEngine;

public class ActionLineBox : BaseStageUI
{
    [SerializeField] private TMP_Text text;

    public IEnumerator TypingCoroutine(string text)
    {
        if (text == null || text == string.Empty) { Hide(); yield break; }

        Show();

        this.text.text = text;

        yield return new WaitForSeconds(1.5f);

        this.text.text = string.Empty;

        Hide();
    }
}
