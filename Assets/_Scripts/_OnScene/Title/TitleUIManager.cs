using UnityEngine;
using UnityEngine.UI;

public class TitleUIManager : MonoBehaviour
{
    public int slotIndex = 1;

    public Button newGameButton;
    public Button loadGameButton;
    public Button settingButton;
    public Button quitButton;

    private void Awake()
    {
        newGameButton.onClick.AddListener(() => OnClickNewGame());
        loadGameButton.onClick.AddListener(() => OnClickLoad());
        settingButton.onClick.AddListener(() => OnClickSetting());
        quitButton.onClick.AddListener(() => OnClickQuit());
    }

    public void OnClickLoad()
    {
        GameManager.Instance.ChangeScene("Home Scene");
    }
    public void OnClickNewGame()
    {
        StoryManager.Instance.SetPrologueEpisode();

        GameManager.Instance.ChangeScene("Stage Scene");
    }
    public void OnClickSetting() { }
    public void OnClickQuit() { }
}
