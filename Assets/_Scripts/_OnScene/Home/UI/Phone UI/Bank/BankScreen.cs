using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BankScreen : BaseScreen
{
    [SerializeField] private TMP_Text moneyText;

    private void OnEnable()
    {
        DataManager.Instance.Play.onMoneyChanged += UpdateMoneyText;
    }

    private void OnDisable()
    {
        DataManager.Instance.Play.onMoneyChanged -= UpdateMoneyText;
    }

    public override void Show()
    {
        base.Show();

        UpdateMoneyText(DataManager.Instance.Play.money);
    }

    private void UpdateMoneyText(int text)
    {
        moneyText.text = $"Money: {text}";
    }
}
