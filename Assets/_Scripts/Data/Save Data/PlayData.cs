using System;
using UnityEngine;

public class PlayData
{
    [field: NonSerialized]
    public event Action<int> onMoneyChanged;

    public int money;
    public DateTime playDate;
    public long playTime;

    public PlayData()
    {
        money = 10000;
        playDate = DateTime.Now;
        playTime = 0;
    }

    public void SetMoney(int value)
    {
        money = value;

        onMoneyChanged?.Invoke(money);
    }
    public void AddMoney(int value)
    {
        money = value;

        onMoneyChanged?.Invoke(money);
    }
}
