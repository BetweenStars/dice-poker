using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class RoundData
{
    public int pot;
    public int ante;
    public int[] dice = new int[2];
}

public static class BettingConfigs
{
    public const int RAISESTEP = 100;
    public const float VIEW_DURATION = 1f;
}

[Flags]
public enum BetAction
{
    None = 0,
    Call = 1 << 1,
    Check = 1 << 2,
    Raise = 1 << 3,
    AllIn = 1 << 4,
    Fold = 1 << 5
}

public class StageNodeController : MonoBehaviour, ILoadingAsyncInitializable
{
    #region events
    public static Action<EpisodeAsset> OnStageChanged;
    public static Action<string> OnNodeChanged;
    #endregion

    #region controllers
    [SerializeField] private StageDialogueController dialogueController;
    [SerializeField] private TableController tableController;
    #endregion

    #region stage fields
    private StageData currentStage;
    #endregion
    #region node fields
    private EpisodeAsset currentEpisode;

    private Dictionary<string, BaseStageNodeData> _nodes = new();
    private BaseStageNodeData currentNode = null;
    private bool isNodePlaying = false;
    #endregion

    #region round fields
    public RoundData roundData { get; private set; } = new();
    private Player[] players = new Player[2];

    [SerializeField] private Player realPlayer;
    [SerializeField] private AIPlayer aiPlayer;
    private Player winner;

    public int bettingPlayerIndex;
    public int rollingPlayerIndex;

    public bool hasAllIn;
    #endregion

    [SerializeField] private Dice firstDice;
    [SerializeField] private Dice secondDice;

    #region initialize
    public void InitializeOnLoading()
    {
        Initialize();

        Reset();
    }
    public void OnReadyToStart()
    {
        PlayStartNode();
    }
    private void Initialize()
    {
        //node controller init
        InitializeData();
        InitializeRuntimeNodes();

        //other controller init
        StageUIManager.Instance.Initialize();
        tableController.Initialize();
        dialogueController.Initialize();
    }
    private void InitializeData()
    {
        //init stage data
        currentStage = StoryManager.Instance.SelectedStage;
        currentEpisode = ResourceManager.Instance.GetEpisodeAsset(DataManager.Instance.Progress.Key);

        //init player
        realPlayer.SetMoney(currentStage.startingMoney);
        aiPlayer.SetMoney(currentStage.startingMoney);

        players[0] = realPlayer;
        players[1] = aiPlayer;

        //init round data
        SetAnte(currentStage.ante);
    }
    private void InitializeRuntimeNodes()
    {
        if (currentEpisode == null) return;

        _nodes.Clear();

        _nodes.Add(currentEpisode.startNodeData.nodeGuid, currentEpisode.startNodeData);

        foreach (var node in currentEpisode.dialogueNodes)
            _nodes.Add(node.nodeGuid, node);

        foreach (var node in currentEpisode.choiceNodes)
            _nodes.Add(node.nodeGuid, node);

        foreach (var node in currentEpisode.spriteNodes)
            _nodes.Add(node.nodeGuid, node);

        foreach (var node in currentEpisode.audioNodes)
            _nodes.Add(node.nodeGuid, node);

        foreach (var node in currentEpisode.passOnlyNodes)
            _nodes.Add(node.nodeGuid, node);

        foreach (var node in currentEpisode.endNodes)
            _nodes.Add(node.nodeGuid, node);

    }
    #endregion

    #region reset
    private void Reset()
    {
        //node controller reset
        winner = null;
        hasAllIn = false;

        SetPot(0);

        bettingPlayerIndex = UnityEngine.Random.Range(0, 2);
        rollingPlayerIndex = (bettingPlayerIndex + 1) % 2;

        rollIndex = 0;

        //other controller reset
        StageUIManager.Instance.Reset();
        dialogueController.Reset();
        tableController.Reset();
    }
    #endregion

    #region basic-node
    private void PlayStartNode()
    {
        if (currentEpisode == null) { Debug.LogWarning("No Stage Container"); return; }
        if (currentEpisode.startNodeData == null) { Debug.LogWarning("No Start Node in Data"); return; }

        currentNode = currentEpisode.startNodeData;

        OnStageChanged?.Invoke(currentEpisode);
        OnNodeChanged?.Invoke(currentNode.nodeGuid);

        isNodePlaying = false;

        PlayNextNode(currentEpisode.startNodeData.nextNodeGuid);
    }

    private void PlayNextNode(string guid)
    {
        if (isNodePlaying) return;

        if (!_nodes.TryGetValue(guid, out currentNode))
        {
            Debug.LogWarning($"[PlayNextNode] 해당 GUID의 노드를 찾을 수 없습니다: {guid}");
            return;
        }

        OnStageChanged?.Invoke(currentEpisode);
        OnNodeChanged?.Invoke(currentNode.nodeGuid);

        StageUIManager.Instance.HideAllDialougeBox();

        // 현재 노드의 타입에 따라 알맞은 인게임 코루틴 연출 분기
        switch (currentNode.nodeType)
        {
            case StageNodeType.Dialogue:
                {
                    var dialogueData = currentNode as DialogueNodeData;
                    StartCoroutine(DialogueCoroutine(dialogueData));
                    break;
                }
            case StageNodeType.Choice:
                {
                    StartCoroutine(ChoiceCoroutine());
                    break;
                }
            case StageNodeType.Sprite:
                {
                    StartCoroutine(SpriteCoroutine());
                    break;
                }
            case StageNodeType.Audio:
                {
                    StartCoroutine(AudioCoroutine());
                    break;
                }

            case StageNodeType.Ante:
                {
                    StartCoroutine(AnteCoroutine());
                    break;
                }
            case StageNodeType.Deal:
                {
                    StartCoroutine(DealCoroutine());
                    break;
                }
            case StageNodeType.Draw:
                {
                    StartCoroutine(DrawCoroutine());
                    break;
                }
            case StageNodeType.Bet:
                {
                    StartCoroutine(BetCoroutine());
                    break;
                }
            case StageNodeType.Roll:
                {
                    StartCoroutine(RollCoroutine());
                    break;
                }
            case StageNodeType.Open:
                {
                    StartCoroutine(OpenCoroutine());
                    break;
                }
            case StageNodeType.Showdown:
                {
                    StartCoroutine(ShowdownCoroutine());
                    break;
                }
            case StageNodeType.ReturnHome:
                {
                    StartCoroutine(ReturnHomeCoroutine());
                    break;
                }
        }
    }

    private void PlayNodeByType(StageNodeType type)
    {
        var result = _nodes.FirstOrDefault(n => n.Value.nodeType == type);

        if (result.Key != null)
        {
            PlayNextNode(result.Value.nodeGuid);
        }
        else { Debug.LogWarning("Can't find key by type"); }
    }

    private string GetNextNodeGuid(int index = 0)
    {
        if (currentNode is StartNodeData stNode) return stNode.nextNodeGuid;
        else if (currentNode is DialogueNodeData dlNode) return dlNode.nextNodeGuid;
        else if (currentNode is PassOnlyNodeData poNode) return poNode.nextNodeGuid;
        else if (currentNode is ChoiceNodeData chNode) return chNode.choiceOptions[index].nextNodeGuid;
        else if (currentNode is SpriteNodeData spData) return spData.nextNodeGuid;
        else if (currentNode is AudioNodeData adData) return adData.nextNodeGuid;
        return string.Empty;
    }
    #endregion

    #region dialogue-node-function
    private IEnumerator DialogueCoroutine(DialogueNodeData data)
    {
        isNodePlaying = true;

        int index = dialogueController.GetPositionByCharId(data.charId);

        yield return StageUIManager.Instance.TypeText(index, data.charId, data.text);

        isNodePlaying = false;
    }

    public void OnClickNextButton()
    {
        bool skipped = StageUIManager.Instance.TrySkipTyping();

        if (skipped) return;

        if (isNodePlaying) return;

        PlayNextNode(GetNextNodeGuid());
    }

    private int choiceIndex = -1;
    private IEnumerator ChoiceCoroutine()
    {
        isNodePlaying = true;
        choiceIndex = -1;

        List<ChoiceOptionData> options = (currentNode as ChoiceNodeData).choiceOptions;
        StageUIManager.Instance.UpdateChoiceButtonsUI(options);

        while (choiceIndex == -1)
        {
            yield return null;
        }

        StageUIManager.Instance.UpdateChoiceButtonsUI(null);

        isNodePlaying = false;
        PlayNextNode(GetNextNodeGuid(choiceIndex));
    }
    public void OnClickChoiceButton(int index)
    {
        choiceIndex = index;
    }

    public IEnumerator AudioCoroutine()
    {
        isNodePlaying = true;
        AudioNodeData adData = currentNode as AudioNodeData;

        if (adData == null)
        {
            Debug.LogWarning("No audio node data in current node");
            isNodePlaying = false;
            yield break;
        }

        switch (adData.audioType)
        {
            case StageAudioType.BGM:
                {
                    //브금 실행
                    AudioManager.Instance.PlayBGMAsync(adData.clipRef)
                    .Forget(exception => Debug.LogError(exception));

                    break;
                }
            case StageAudioType.SFX:
                {
                    break;
                }
        }

        isNodePlaying = false;
        PlayNextNode(adData.nextNodeGuid);
    }

    public IEnumerator SpriteCoroutine()
    {
        isNodePlaying = true;

        SpriteNodeData spData = currentNode as SpriteNodeData;

        if (spData == null)
        {
            Debug.LogWarning("No sprite node data in current node");
            isNodePlaying = false;
            yield break;
        }

        switch (spData.spriteAction)
        {
            case SpriteActionType.Show:
                {
                    dialogueController.ShowSlot(spData.charId, spData.spriteId, spData.position);
                    break;
                }
            case SpriteActionType.Hide:
                {
                    dialogueController.HideSlot(spData.charId);
                    break;
                }
            case SpriteActionType.Move:
                {
                    dialogueController.MoveSlot(spData.charId, spData.position, 0.5f);
                    break;
                }
            case SpriteActionType.Sprite:
                {
                    dialogueController.ChangeSprite(spData.charId, spData.spriteId);
                    break;
                }
            default:
                {
                    Debug.Log("not supported sprite action type");
                    break;
                }
        }


        isNodePlaying = false;
        PlayNextNode(GetNextNodeGuid());
        yield return null;
    }
    #endregion

    #region game-node
    private IEnumerator AnteCoroutine()
    {
        isNodePlaying = true;

        Reset();

        yield return SetView(CameraState.Betting, BettingConfigs.VIEW_DURATION);

        foreach (Player player in players)
        {
            player.AddMoney(-roundData.ante);
            AddPot(roundData.ante);

            yield return new WaitForSeconds(0.5f);
        }

        isNodePlaying = false;
        PlayNextNode(GetNextNodeGuid());
    }

    private IEnumerator DealCoroutine()
    {
        isNodePlaying = true;

        //yield return SetView(CameraState.Table, BettingConfigs.VIEW_DURATION);

        yield return tableController.ShuffleDeck();

        foreach (var player in players)
        {
            yield return tableController.DrawCard(player);
        }
        foreach (var player in players)
        {
            yield return tableController.DrawCard(player);
        }

        if (!hasAllIn)
        {
            yield return SetView(CameraState.Betting, BettingConfigs.VIEW_DURATION);
        }

        isNodePlaying = false;
        PlayNextNode(GetNextNodeGuid());
    }
    private IEnumerator DrawCoroutine()
    {
        isNodePlaying = true;

        yield return SetView(CameraState.Table, BettingConfigs.VIEW_DURATION);

        foreach (var player in players)
        {
            yield return tableController.DrawCard(player);
        }

        if (!hasAllIn)
        {
            yield return SetView(CameraState.Betting, BettingConfigs.VIEW_DURATION);
        }

        isNodePlaying = false;
        PlayNextNode(GetNextNodeGuid());
    }

    private class BetState
    {
        public int playerIndex;
        public bool isBetEnd;
        public bool hasFoldedPlayer;

        public int currentBet;
        public int lastRasieAmount;

        public bool hasAllInPlayer;
        public Player AllInPlayer;

        public BetState(int playerIndex, int currentBet, int lastRaiseAmount)
        {
            this.playerIndex = playerIndex;
            isBetEnd = false;
            hasFoldedPlayer = false;
            this.currentBet = currentBet;
            this.lastRasieAmount = lastRaiseAmount;
            hasAllInPlayer = false;
            AllInPlayer = null;
        }

        public void Init() { }

        public int GetMinRaise()
        {
            return currentBet + lastRasieAmount;
        }
        public void SetLastRaiseAmount(int value)
        {
            this.lastRasieAmount = value;
        }
    }

    private IEnumerator BetCoroutine()
    {
        isNodePlaying = true;

        if (hasAllIn)
        {
            isNodePlaying = false;

            PlayNextNode(GetNextNodeGuid());

            yield break;
        }

        yield return SetView(CameraState.Betting, BettingConfigs.VIEW_DURATION);

        BetState betState = new(bettingPlayerIndex, 0, BettingConfigs.RAISESTEP);

        foreach (var p in players) { p.InitBetStates(); }
        StageUIManager.Instance.UpdateBetTexts(players[0].currentBet, players[1].currentBet);

        while (!betState.isBetEnd)
        {
            Player player = players[betState.playerIndex];

            player.availableBetAction = CheckAvailableBetActions(betState, player);
            //BetAction이 None이면 오류
            if (player.availableBetAction == BetAction.None) { Debug.LogWarning("player Has No available Action"); }

            if (player is AIPlayer)
            {
                float winRate = HandEvaluator.GetHandWinRate(player.cardsInHand, tableController.GetDeckData(),
                rollIndex > 0 ? roundData.dice[0] : null, rollIndex > 1 ? roundData.dice[1] : null);

                (player as AIPlayer).winRate = winRate;

                yield return player.WaitForBetCoroutine();
            }
            else
            {
                StageUIManager.Instance.BetUI.Show();
                StageUIManager.Instance.BetUI.raiseSlider.UpdateRaiseSlider(betState.GetMinRaise(), player.money + player.currentBet);
                StageUIManager.Instance.BetUI.EnableAvailableButtons(player.availableBetAction);

                yield return player.WaitForBetCoroutine();

                aiPlayer.ToastReactionLine(player.selectedBetAction);

                StageUIManager.Instance.BetUI.Hide();
            }

            switch (player.selectedBetAction)
            {
                case BetAction.Check:
                    {
                        betState.isBetEnd = players.All(p => betState.currentBet == p.currentBet && p.selectedBetAction != BetAction.None);
                        break;
                    }
                case BetAction.Call:
                    {
                        player.needMoney = betState.currentBet - player.currentBet;
                        player.currentBet = betState.currentBet;
                        player.AddMoney(-player.needMoney);
                        AddPot(player.needMoney);

                        betState.isBetEnd = players.All(p => betState.currentBet == p.currentBet && p.selectedBetAction != BetAction.None);
                        break;
                    }
                case BetAction.Raise:
                    {
                        player.SetRaiseValue(betState.GetMinRaise());
                        betState.lastRasieAmount = player.currentBet - betState.currentBet;
                        betState.currentBet = player.currentBet;

                        player.AddMoney(-player.needMoney);

                        AddPot(player.needMoney);

                        players[(betState.playerIndex + 1) % 2].ResetAction();

                        break;
                    }
                case BetAction.AllIn:
                    {
                        yield return StageUIManager.Instance.ShowAllInCutScene();

                        player.needMoney = player.money;
                        player.currentBet = player.currentBet + player.needMoney;
                        player.AddMoney(-player.needMoney);
                        AddPot(player.needMoney);

                        betState.hasAllInPlayer = true;
                        betState.AllInPlayer = player;

                        if (player.currentBet > betState.currentBet)
                        {
                            betState.lastRasieAmount = player.currentBet - betState.currentBet;
                            betState.currentBet = player.currentBet;
                            players[(betState.playerIndex + 1) % 2].ResetAction();
                        }

                        betState.isBetEnd = players.All(p => betState.currentBet == p.currentBet && p.selectedBetAction != BetAction.None);
                        break;
                    }
                case BetAction.Fold:
                    {
                        betState.isBetEnd = true;
                        betState.hasFoldedPlayer = true;
                        winner = players[(betState.playerIndex + 1) % 2];
                        break;
                    }
            }
            StageUIManager.Instance.UpdateBetTexts(players[0].currentBet, players[1].currentBet);
            if (!betState.isBetEnd) betState.playerIndex = (betState.playerIndex + 1) % 2;
        }

        hasAllIn = players.Any(p => p.selectedBetAction == BetAction.AllIn) &&
        players.All(p => p.selectedBetAction == BetAction.AllIn || p.selectedBetAction == BetAction.Call);

        StageUIManager.Instance.UpdateBetTexts(0, 0);
        dialogueController.ChangeSprite(aiPlayer.GetProfile().id, "idle");
        yield return new WaitForSeconds(1.2f);

        isNodePlaying = false;

        if (betState.hasFoldedPlayer) { PlayNodeByType(StageNodeType.Showdown); }
        else { PlayNextNode(GetNextNodeGuid()); }
    }

    private BetAction CheckAvailableBetActions(BetState state, Player player)
    {
        if (state.AllInPlayer == player) return BetAction.None;

        int toCall = state.currentBet - player.currentBet;
        int raiseCost = state.GetMinRaise() - player.currentBet;

        if (state.hasAllInPlayer)
        {
            if (toCall >= player.money) { return BetAction.Fold | BetAction.AllIn; }
            else { return BetAction.Fold | BetAction.Call; }
        }

        if (toCall == 0)
        {
            if (player.money < raiseCost) { return BetAction.Check | BetAction.AllIn; }
            else { return BetAction.Check | BetAction.Raise | BetAction.AllIn; }
        }

        if (toCall > 0)
        {
            if (toCall <= player.money)
            {
                BetAction action = BetAction.Fold | BetAction.Call | BetAction.AllIn;

                if (player.money >= raiseCost) { action |= BetAction.Raise; }

                return action;
            }
            else { return BetAction.Fold | BetAction.AllIn; }
        }

        return BetAction.None;
    }

    private int rollIndex = 0;
    private bool isRolled = false;
    private IEnumerator RollCoroutine()
    {
        if (rollIndex > 1 || rollIndex < 0) { Debug.Log($"roll index out of range. roll index : {rollIndex}"); yield break; }
        isNodePlaying = true;
        isRolled = false;

        yield return SetView(CameraState.Table, BettingConfigs.VIEW_DURATION);

        //현재 굴릴 주사위 자리로 이동
        yield return tableController.MoveDiceToThrowPosition(rollIndex, rollingPlayerIndex);

        //현재 굴리는 플레이어에 따라
        if (rollingPlayerIndex == 0)
        {
            StageUIManager.Instance.EnableRollButton();
        }
        else if (rollingPlayerIndex == 1)
        {
            yield return new WaitForSeconds(0.7f);
            isRolled = true;
        }

        //굴리기 기다리기
        while (!isRolled)
        {
            yield return null;
        }

        //굴리면 버튼 비활성화 - ai면 활성화 안되어있지만 만약을 위해 비활성화
        StageUIManager.Instance.DisableRollButton();

        //굴리고 기다리기
        yield return tableController.RollDice(rollIndex);

        roundData.dice[rollIndex] = tableController.GetDiceValue(rollIndex);

        StageUIManager.Instance.UpdateDiceUI(rollIndex, roundData.dice[rollIndex]);
        rollingPlayerIndex = (rollingPlayerIndex + 1) % 2;
        rollIndex++;

        if (!hasAllIn)
        {
            yield return SetView(CameraState.Betting, BettingConfigs.VIEW_DURATION);
        }

        isNodePlaying = false;
        PlayNextNode(GetNextNodeGuid());
    }

    public void OnClickRollButton()
    {
        isRolled = true;
    }

    private IEnumerator OpenCoroutine()
    {
        isNodePlaying = true;

        yield return null;

        isNodePlaying = false;
        PlayNextNode(GetNextNodeGuid());
    }
    private IEnumerator ShowdownCoroutine()
    {
        isNodePlaying = true;

        bool hasFoldedPlayer = false;

        foreach (var p in players)
        {
            if (p.selectedBetAction == BetAction.Fold) { hasFoldedPlayer = true; }

            p.handResult = HandEvaluator.Evaluate(p.cardsInHand, roundData.dice[0], roundData.dice[1]);
        }

        if (!hasFoldedPlayer || winner == null) { winner = HandEvaluator.CompareHands(players[0], players[1]); }

        Debug.Log($"Player has {realPlayer.handResult.handRank}, AI has {aiPlayer.handResult.handRank}");

        if (winner != null)
        {
            winner.AddMoney(roundData.pot);
            Debug.Log("Winner: " + winner.name);
            yield return null;
        }
        else
        {
            Debug.Log("Draw! Pot is split.");

            // 무승부인 경우, 팟을 나눠 가짐
            foreach (var player in players)
            {
                player.AddMoney(roundData.pot / players.Length);
            }
            yield return null;
        }

        SetPot(0);

        yield return new WaitForSeconds(1f);

        foreach (var player in players)
        {
            player.ResetCards();
        }

        yield return tableController.ClearTable();

        yield return new WaitForSeconds(1.2f);

        yield return SetView(CameraState.Dialogue, BettingConfigs.VIEW_DURATION);

        isNodePlaying = false;
        PlayNextNode(GetNextNodeGuid());
    }
    #endregion

    #region other-node
    private IEnumerator ReturnHomeCoroutine()
    {
        isNodePlaying = true;

        foreach (var player in players)
        {
            if (player is not AIPlayer)
            {
                DataManager.Instance.Play.AddMoney(player.money);
            }
        }

        yield return new WaitForSeconds(1f);

        //foreach (var flag in currentEpisode.addFlags) { ScenarioManager.Instance.AddFlag(flag); }
        //foreach (var flag in currentEpisode.removeFlages) { ScenarioManager.Instance.RemoveFlag(flag); }

        //ScenarioManager.Instance.AddCompletedEventId(currentEpisode.eventId);
        //if (currentEpisode.priority == 0) ScenarioManager.Instance.AdvanceRouteIndex();

        OnNodeChanged?.Invoke(null);

        isNodePlaying = false;
        GameManager.Instance.ChangeScene("Home Scene");
    }
    #endregion

    #region round-data
    private void SetPot(int value)
    {
        roundData.pot = value;
        StageUIManager.Instance.UpdatePotText(roundData.pot);
    }
    private void AddPot(int value)
    {
        roundData.pot += value;
        StageUIManager.Instance.UpdatePotText(roundData.pot);
    }
    private void SetAnte(int value)
    {
        roundData.ante = value;
        StageUIManager.Instance.UpdateAnteText(roundData.ante);
    }
    #endregion

    #region camera
    private IEnumerator SetView(CameraState state, float duration)
    {
        StageCameraManager.Instance.SetCamera(state, duration);
        StartCoroutine(dialogueController.MoveViewCoroutine(state, duration));
        //맵, 플레이어 이동
        yield return new WaitForSeconds(duration);
    }
    #endregion
}