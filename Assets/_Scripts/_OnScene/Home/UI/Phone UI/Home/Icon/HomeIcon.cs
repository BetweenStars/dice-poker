using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HomeIcon : MonoBehaviour
{
    [SerializeField] protected BaseScreen targetScreen;
    private Button button;

    protected virtual void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    protected virtual void OnClick()
    {
        PhoneManager.Instance.OpenTargetScreen(targetScreen);
    }
}
