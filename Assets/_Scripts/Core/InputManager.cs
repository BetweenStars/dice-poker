using UnityEngine;
using UnityEngine.InputSystem;
public class InputManager:MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    //나중에 inputactions에서 참조로 가져오게 수정
    public InputActionAsset inputActions;
    public InputActionReference scrollAction;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        scrollAction.action.Enable();
    }
}