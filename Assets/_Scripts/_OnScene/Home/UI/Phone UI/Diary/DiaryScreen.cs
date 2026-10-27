using System;
using System.Collections.Generic;
using UnityEngine;

public class DiaryScreen : BaseScreen
{
    [SerializeField] private DiarySlot[] diarySlots = new DiarySlot[5];

    protected override void Awake()
    {
        base.Awake();

        DataManager.Instance.SaveData(0);
    }

    public override void Show()
    {
        //슬롯들 데이터 갱신
        for (int i = 0; i < diarySlots.Length; i++)
        {
            diarySlots[i].Initialize(i);
        }

        base.Show();
    }
}