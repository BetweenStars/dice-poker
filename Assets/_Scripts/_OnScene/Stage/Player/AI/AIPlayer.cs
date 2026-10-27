using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AIPlayer : Player
{
    [SerializeField] private CharacterAsset profile;

    #region dialogue fields
    [SerializeField] private StageDialogueController controller;
    #endregion

    #region auto bet fields
    public float winRate;

    public float callScore = 0f;
    public float checkScore = 0f;
    public float raiseScore = 0f;
    public float allInScore = 0f;
    public float foldScore = 0f;
    #endregion

    #region betting
    //wrapped function
    private void ToastActionLine() => StageUIManager.Instance.ToastActionLine(profile.ReactionLine.GetActionLine(selectedBetAction));
    public void ToastReactionLine(BetAction action) => StageUIManager.Instance.ToastActionLine(profile.ReactionLine.GetReactionLine(action));
    public override void SelectRaise()
    {
        Debug.Log(name + " select Raise");

        isActionSelected = true;
        selectedBetAction = BetAction.Raise;

        controller.ChangeSprite(profile.id, "happy");
    }

    public override void SetRaiseValue(int min)
    {
        //가능 행동 체크할때 한번 거르긴 하는데 혹시 몰라서
        if (currentBet + money < min) { Debug.LogWarning("Cannot Raise With this Money"); }

        //나중에 가중치에 따라 레이즈값 높게 낮게
        int rand = UnityEngine.Random.Range(min, currentBet + money);

        int maxRaise = (currentBet + money - 1) / BettingConfigs.RAISESTEP * BettingConfigs.RAISESTEP;

        int nextBet = Mathf.Clamp(Mathf.RoundToInt(rand / BettingConfigs.RAISESTEP) * BettingConfigs.RAISESTEP,
        min, maxRaise);

        needMoney = nextBet - currentBet;

        currentBet = nextBet;
    }

    public override IEnumerator WaitForBetCoroutine()
    {
        Debug.Log("Win Rate : " + winRate);

        controller.ChangeSprite(profile.id, "talk");

        float thinkDuration = UnityEngine.Random.Range(1.0f, 3.5f);
        yield return new WaitForSeconds(thinkDuration);

        SetBetActionByScore();
        ToastActionLine();
    }

    private void SetBetActionByScore()
    {
        checkScore = ((availableBetAction & BetAction.Check) != 0) ? 50f : 0f;
        callScore = ((availableBetAction & BetAction.Call) != 0) ? 50f : 0f;
        raiseScore = ((availableBetAction & BetAction.Raise) != 0) ? 10f : 0f;
        allInScore = ((availableBetAction & BetAction.AllIn) != 0) ? 1f : 0f;
        foldScore = ((availableBetAction & BetAction.Fold) != 0) ? 5f : 0f;

        float totalScore = checkScore + callScore + raiseScore + allInScore + foldScore;
        float randScore = UnityEngine.Random.Range(0f, totalScore);

        float current = 0f;

        current += checkScore;
        if (randScore < current)
        {
            SelectCheck();
            return;
        }

        current += callScore;
        if (randScore < current)
        {
            SelectCall();
            return;
        }

        current += raiseScore;
        if (randScore < current)
        {
            SelectRaise();
            return;
        }

        current += allInScore;
        if (randScore < current)
        {
            SelectAllIn();
            return;
        }

        SelectFold();
    }
    #endregion

    #region set-profile
    public void SetProfile(CharacterAsset newProfile) => profile = newProfile;
    public CharacterAsset GetProfile() => profile;
    #endregion
}
