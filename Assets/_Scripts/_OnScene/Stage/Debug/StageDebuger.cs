using UnityEngine;

public class StageDebuger : BaseStageUI
{
    public void OnClickReturnHomeButton()
    {
        GameManager.Instance.ChangeScene("Home Scene");
    }

    public void OnClickCloseDebugTool() { Hide(); }
    public void OnClickOpenDebugTool() { Show(); }

    public void QuitGame()
    {
        Application.Quit();
    }
}
