using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 인덱스 0번은 자동저장 칸
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class DiarySlot : MonoBehaviour
{
    private CanvasGroup cg;
    [SerializeField] private int index;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text playTimeText;
    [SerializeField] private TMP_Text playDateText;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button deleteButton;

    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();

        if (index != 0)
        {
            loadButton.gameObject.SetActive(false);
            deleteButton.gameObject.SetActive(false);

            saveButton.onClick.AddListener(() => OnClickSave());
            deleteButton.onClick.AddListener(() => OnClickDelete());
        }

        loadButton.onClick.AddListener(async () => await OnClickLoad());
    }

    private void OnClickSave()
    {
        DataManager.Instance.SaveData(index);
        Initialize(index);
    }
    private async UniTask OnClickLoad()
    {
        await DataManager.Instance.LoadData(index);
    }
    private void OnClickDelete()
    {
        DataUtility.Delete(index);
        Initialize(index);
    }

    public void Initialize(int index)
    {
        this.index = index;

        if (index == 0)
        {
            titleText.text = "Auto";
        }
        else
        {
            titleText.text = $"Save{index}";
        }

        if (DataUtility.HasData(index))
        {
            GameData data = DataUtility.Load(index);

            if (index != 0)
            {
                loadButton.gameObject.SetActive(true);
                deleteButton.gameObject.SetActive(true);
            }

            playDateText.text = data.Play.playDate.ToString("yyyy-MM-dd");
            TimeSpan playTime = TimeSpan.FromSeconds(data.Play.playTime);
            playTimeText.text = $"{(int)playTime.TotalHours:00}:{playTime.Minutes:00}:{playTime.Seconds:00}";
        }
        else
        {
            if (index != 0)
            {
                loadButton.gameObject.SetActive(false);
                deleteButton.gameObject.SetActive(false);
            }
            playDateText.text = string.Empty;
            playTimeText.text = string.Empty;
        }
    }
}