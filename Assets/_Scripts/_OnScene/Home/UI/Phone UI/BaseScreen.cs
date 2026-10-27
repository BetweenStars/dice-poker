using UnityEngine;
using System.Linq;

[RequireComponent(typeof(CanvasGroup))]
public class BaseScreen : MonoBehaviour
{
    private IDataSyncable[] _syncables;
    private CanvasGroup canvasGroup;

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        Hide();
    }

    public virtual void Show()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        RefreshScreen();
    }

    public virtual void Hide()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    protected virtual void RefreshScreen()
    {
        _syncables = GetComponentsInChildren<IDataSyncable>().ToArray();
        
        foreach (var syncable in _syncables) { syncable.SyncData(); }
    }
}
