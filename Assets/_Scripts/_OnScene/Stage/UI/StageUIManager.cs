using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageUIManager : MonoBehaviour
{
    public static StageUIManager Instance { get; private set; }

    public Image BackGroundImage;

    public StageHUDs HUD;
    public BetUIs BetUI;
    public DialougeUIs DialougeUI;
    public ChoiceUIs ChoiceUI;
    public CutSceneUIs CutSceneUI;

    public ActionLineBox actionLineUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    #region wrapping functions
    public void UpdateDiceUI(int index, int value) => HUD.UpdateDice(index, value);
    /// <summary>
    /// 인덱스가 -1이면 초기화
    /// </summary>
    /// <param name="index">카드 인덱스</param>
    /// <param name="cardSprite">카드 스프라이트</param>
    public void UpdateCardUI(int index, Sprite sprite = null) => HUD.UpdateCardUI(index, sprite);
    public void UpdatePlayerMoneyUI(int value) => HUD.UpdatePlayerMoney(value);
    public void UpdateAIMoneyUI(int value) => HUD.UpdateAIMoney(value);

    public void UpdateChoiceButtonsUI(List<ChoiceOptionData> options) => ChoiceUI.UpdateChoiceButtons(options);
    /// <summary>
    /// value1은 플레이어 베팅금, value2는 ai 베팅금
    /// </summary>
    /// <param name="value1">플레이어</param>
    /// <param name="value2">AI</param>
    public void UpdateBetTexts(int value1, int value2) => BetUI.UpdateBetTexts(value1, value2);

    public void UpdatePotText(int value) => HUD.roundInfoUi.UpdatePotText(value);
    public void UpdateAnteText(int value) => HUD.roundInfoUi.UpdateAnteText(value);
    public void EnableRollButton() => HUD.rollButton.Show();
    public void DisableRollButton() => HUD.rollButton.Hide();
    /// <summary>
    /// dialogue controller에서 받은 인텍스 1~5를 -1해서 배열 값으로 전달
    /// </summary>
    /// <param name="index"></param>
    /// <param name="charId"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public System.Collections.IEnumerator TypeText(int index, CharacterId charId, string text)
    {
        if (index < 1 || index > 5) index = 3;
        yield return DialougeUI.dialogueBoxes[index - 1].TypeText(charId, text);
    }
    public bool TrySkipTyping()
    {
        foreach (var box in DialougeUI.dialogueBoxes)
        {
            if (box.IsTyping()) { box.SkipTyping(); return true; }
        }
        return false;
    }
    public void HideAllDialougeBox() => DialougeUI.HideAllDialgueBox();

    public System.Collections.IEnumerator ShowAllInCutScene() => CutSceneUI.AllInCutScene.ShowCutScene();
    #endregion

    #region action-line-function
    public void ToastActionLine(string text)
    {
        StartCoroutine(actionLineUI.TypingCoroutine(text));
    }
    #endregion

    #region initialize
    public void Initialize()
    {
        BackGroundImage.sprite = ResourceManager.Instance.GetBackGroundSprite();
    }
    public void Reset()
    {
        foreach (var cardui in HUD.cardUIs)
        {
            cardui.Hide();
        }
        DialougeUI.HideAllDialgueBox();
        BetUI.Hide();
        UpdateChoiceButtonsUI(null);
        HUD.UpdateCardUI(-1);
        actionLineUI.Hide();
    }
    #endregion
}