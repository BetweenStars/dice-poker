using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using NUnit.Framework;

public class DialogueBox : BaseStageUI
{
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private CanvasGroup indicator;

    private Coroutine indicatorCoroutine;

    public override void Show()
    {
        canvasGroup.alpha = 1;
        indicator.alpha = 0;
    }

    public override void Hide()
    {
        if (indicatorCoroutine != null)
        {
            StopCoroutine(indicatorCoroutine);
            indicatorCoroutine = null;
        }

        base.Hide();
    }

    private float blinkDuration = 0.5f;
    private bool isTyping = false;
    private string currentText;
    public IEnumerator TypeText(CharacterId charId, string text)
    {
        isTyping = true;
        dialogueText.text = string.Empty;
        Show();

        speakerText.text = CharacterIdUtility.GetString(charId);
        currentText = text;

        foreach (char c in currentText)
        {
            if (dialogueText.text == currentText) break;

            dialogueText.text += c;
            yield return new WaitForSeconds(DataManager.Instance.Setting.TypingSpeed);
        }

        isTyping = false;
        indicatorCoroutine = StartCoroutine(BlinkContinueIndicator());
    }

    public void SkipTyping()
    {
        if (!isTyping) return;

        dialogueText.text = currentText;
        isTyping = false;
    }

    public bool IsTyping() => isTyping;

    private IEnumerator BlinkContinueIndicator()
    {
        float blinkSpeed = 1f / blinkDuration;

        bool fadeIn = true;

        while (!isTyping)
        {
            if (fadeIn)
            {
                indicator.alpha -= blinkSpeed * Time.deltaTime;

                if (indicator.alpha <= 0)
                {
                    indicator.alpha = 0;
                    fadeIn = false;
                }
            }
            else
            {
                indicator.alpha += blinkSpeed * Time.deltaTime;
                if (indicator.alpha >= 1)
                {
                    indicator.alpha = 1;
                    fadeIn = true;
                }
            }
            yield return null;
        }
    }
}
