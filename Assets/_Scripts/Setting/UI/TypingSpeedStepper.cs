using System.Collections.Generic;
using UnityEngine;

public class TypingSpeedStepper : MonoBehaviour, IDataSyncable
{
    private Stepper _stepper;

    private void Awake()
    {
        _stepper = GetComponentInChildren<Stepper>();
    }

    private void Start()
    {
        _stepper.SetItems(new List<string>()
        {
           "Slow",
           "Normal",
           "Fast"
        });

        _stepper.OnValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(int index)
    {
        switch (index)
        {
            case 0:
                {
                    SettingManager.Instance.SetTypingSpeed(TypingSpeedConfigs.SLOW);
                    break;
                }
            case 1:
                {
                    SettingManager.Instance.SetTypingSpeed(TypingSpeedConfigs.NORMAL);
                    break;
                }
            case 2:
                {
                    SettingManager.Instance.SetTypingSpeed(TypingSpeedConfigs.FAST);
                    break;
                }
        }

        DataManager.Instance.SaveTypingSpeed();
    }

    public void SyncData()
    {
        float typingSpeed = DataManager.Instance.Setting.TypingSpeed;

        if (Mathf.Approximately(typingSpeed, TypingSpeedConfigs.SLOW)) { _stepper.SetIndex(0); }
        if (Mathf.Approximately(typingSpeed, TypingSpeedConfigs.NORMAL)) { _stepper.SetIndex(1); }
        if (Mathf.Approximately(typingSpeed, TypingSpeedConfigs.FAST)) { _stepper.SetIndex(2); }
    }
}