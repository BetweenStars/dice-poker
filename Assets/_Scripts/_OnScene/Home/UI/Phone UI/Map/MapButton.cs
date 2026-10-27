using UnityEngine;
using UnityEngine.UI;

public class MapButton : MonoBehaviour
{
    [SerializeField] private StageData stageData;
    public EpisodeAsset StageEpisode { get; private set; }

    [SerializeField] private CanvasGroup eventMark;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => OnMapButtonClicked());

        eventMark.alpha = 0;
    }


    private void OnEnable()
    {
        if (StageEpisode == null) { eventMark.alpha = 0; }
        else { eventMark.alpha = 1; }
    }

    public void SetStageEventData(EpisodeAsset eventData)
    {
        if (eventData != null)
        {
            StageEpisode = eventData;
            eventMark.alpha = 1;
        }
        else
        {
            eventMark.alpha = 0;
        }
    }


    private void OnMapButtonClicked()
    {
        /*
        if (stageEventData == null) { ScenarioManager.Instance.SetDefaultStageData(stageData); }
        else { ScenarioManager.Instance.SetStageData(stageData, stageEventData); }
        GameManager.Instance.OpenStageScene(stageData.sceneName);
        */
    }
}