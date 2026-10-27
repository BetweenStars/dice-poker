using System.Collections.Generic;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    #region prologue
    [SerializeField] private StageData _prologueStageData;
    [SerializeField] private EpisodeKey _prologueEpisodeKey;
    public StageData SelectedStage { get; private set; }

    public void SetPrologueEpisode()
    {
        SelectedStage = _prologueStageData;
        DataManager.Instance.Progress.EpisodeKey = _prologueEpisodeKey;
    }
    #endregion

    #region episode
    private List<EpisodeAsset> _availableEpisodes = new();
    public void CheckAvailableEpisode()
    {
        _availableEpisodes.Clear();

        //메인 에피소드 날인지
        //메인 있는 날은 메인만 뜨게

        //메인이 아닌 날은 사이드들이 뜨게
    }
    #endregion
}