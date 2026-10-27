using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SettingScreen : BaseScreen
{
    [SerializeField] private Button _resetButton;

    protected override void Awake()
    {
        base.Awake();

        _resetButton.onClick.AddListener(OnClickResetToDefaults);
    }

    public void OnClickResetToDefaults()
    {
        SettingManager.Instance.ResetToDefaults();

        RefreshScreen();
    }
}