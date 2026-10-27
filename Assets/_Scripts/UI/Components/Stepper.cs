using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
using TMPro;
using System;

/// <summary>
/// 좌우 or 상하 버튼을 눌러서 값을 변경할 수 있는 UI 컴포넌트
/// </summary>
public class Stepper : MonoBehaviour
{
    [SerializeField] private Button _prevButton;
    [SerializeField] private Button _nextButton;

    [SerializeField] private TMP_Text _valueText;

    [SerializeField]
    private List<string> _items = new()
    {
        "Item"
    };

    private int _currentIndex = 0;
    [SerializeField] private bool _loop = false;

    [SerializeField] private UnityEvent<int> _onValueChanged = new();
    public UnityEvent<int> OnValueChanged => _onValueChanged;

    private void Awake()
    {
        _prevButton.onClick.AddListener(OnClickPrev);
        _nextButton.onClick.AddListener(OnClickNext);

        if (_items.Count == 0)
        {
            Debug.LogWarning("Stepper: Items list is empty. Please add items to the list.");
            _valueText.text = string.Empty;
            _prevButton.interactable = false;
            _nextButton.interactable = false;
            return;
        }

        Refresh();
    }

    public void SetItems(List<string> items)
    {
        _items = items;

        _currentIndex = Mathf.Clamp(_currentIndex, 0, _items.Count - 1);

        Refresh();
    }
    public void SetIndex(int index)
    {
        index = Mathf.Clamp(index, 0, _items.Count - 1);

        if (index == _currentIndex) return;

        _currentIndex = index;

        _onValueChanged?.Invoke(_currentIndex);

        Refresh();
    }

    #region onclick-event
    private void OnClickPrev()
    {
        if (_loop)
        {
            SetIndex((_currentIndex - 1 + _items.Count) % _items.Count);
        }
        else
        {
            SetIndex(_currentIndex - 1);
        }
    }
    private void OnClickNext()
    {
        if (_loop)
        {
            SetIndex((_currentIndex + 1) % _items.Count);
        }
        else
        {
            SetIndex(_currentIndex + 1);
        }
    }
    #endregion

    private void Refresh()
    {
        _valueText.text = _items[_currentIndex];

        if (_loop)
        {
            _prevButton.interactable = true;
            _nextButton.interactable = true;
        }
        else
        {
            _prevButton.interactable = _currentIndex > 0;
            _nextButton.interactable = _currentIndex < _items.Count - 1;
        }
    }
}
