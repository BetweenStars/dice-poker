using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    
    [SerializeField] private CanvasGroup fadeImg;

    public async UniTask FadeInAsync(float duration)
    {
        await fadeImg.DOFade(0f, duration).AsyncWaitForCompletion();
    }

    public async UniTask FadeOutAsync(float duration)
    {
        await fadeImg.DOFade(1f, duration).AsyncWaitForCompletion();
    }
}
