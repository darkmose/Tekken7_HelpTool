using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameEvents;
public class TabsSwitcher : MonoBehaviour
{
    private static TabsSwitcher _innerInstance;

    [SerializeField] private Toggle _addPlayersTabToggle;
    [SerializeField] private Toggle _playersTabToggle;
    [SerializeField] private Toggle _battleTabToggle;

    [SerializeField] private Canvas _addPlayerCanvas;
    [SerializeField] private Canvas _playersCanvas;
    [SerializeField] private Canvas _battleCanvas;

    private void Awake()
    {
        if (_innerInstance == null)
        {
            _innerInstance = this;
        }
        _addPlayersTabToggle.onValueChanged.AddListener(OnAddPlayersToggleHandler);        
        _playersTabToggle.onValueChanged.AddListener(OnPlayersToggleHandler);        
        _battleTabToggle.onValueChanged.AddListener(OnBattleToggleHandler);
        EventsAgregator.Subscribe<OnStartGenerateCharsEvent>(OnStartGenerateCharsHandler);
    }

    public static void SetAddPlayerTabInteractable(bool value)
    {
        _innerInstance._addPlayersTabToggle.interactable = value;
        Color color = value ? Color.white : Color.grey;
        _innerInstance._addPlayersTabToggle.image.color = color;
    }
    public static void SetPlayersTabInteractable(bool value)
    {
        _innerInstance._playersTabToggle.interactable = value;
        Color color = value ? Color.white : Color.grey;
        _innerInstance._playersTabToggle.image.color = color;
    }
    public static void SetBattleTabInteractable(bool value)
    {
        _innerInstance._battleTabToggle.interactable = value;
        Color color = value ? Color.white : Color.grey;
        _innerInstance._battleTabToggle.image.color = color;
    }


    private void OnStartGenerateCharsHandler(object sender, OnStartGenerateCharsEvent data)
    {
        SetAddPlayerTabInteractable(false);
    }

    private void OnBattleToggleHandler(bool value)
    {
        _battleCanvas.enabled = value;
    }

    private void OnPlayersToggleHandler(bool value)
    {
        _playersCanvas.enabled = value;
    }

    private void OnAddPlayersToggleHandler(bool value)
    {
        _addPlayerCanvas.enabled = value;
    }

    public static void SwitchTab(int index)
    {
        if (index >= 0 && index < 3)
        {
            switch (index)
            {
                case 0:
                    _innerInstance._addPlayersTabToggle.isOn = true;
                    break;
                case 1:
                    _innerInstance._playersTabToggle.isOn = true;
                    break;
                case 2:
                    _innerInstance._battleTabToggle.isOn = true;
                    break;
            }
        }
    }




}
