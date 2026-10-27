using UnityEngine;

public class Stage : MonoBehaviour
{
    public StageData stageData;

    public void OnStageButtonClicked()
    {
        GameManager.Instance.ChangeScene("Stage Scene");
    }
}
