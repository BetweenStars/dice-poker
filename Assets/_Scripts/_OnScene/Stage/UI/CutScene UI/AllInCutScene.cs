using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class AllInCutScene : BaseStageUI
{
    public Image image;

    public IEnumerator ShowCutScene()
    {
        Show();
        yield return new WaitForSeconds(1.2f);
        Hide();
    }
}