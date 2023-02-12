using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SoftEvents;
public class TabsSwitcher : MonoBehaviour
{
    public enum Tab
    {
        Register,
        Players,
        Battle,
        History
    }

    private static TabsSwitcher _innerInstance;

    [SerializeField] private Toggle _addPlayersTabToggle;
    [SerializeField] private Toggle _playersTabToggle;
    [SerializeField] private Toggle _battleTabToggle;
    [SerializeField] private Toggle _historyTabToggle;

    [SerializeField] private Canvas _addPlayerCanvas;
    [SerializeField] private Canvas _playersCanvas;
    [SerializeField] private Canvas _battleCanvas;
    [SerializeField] private Canvas _historyCanvas;

    private void Awake()
    {
        if (_innerInstance == null)
        {
            _innerInstance = this;
        }
        _addPlayersTabToggle.onValueChanged.AddListener(OnAddPlayersToggleHandler);        
        _playersTabToggle.onValueChanged.AddListener(OnPlayersToggleHandler);        
        _battleTabToggle.onValueChanged.AddListener(OnBattleToggleHandler);
        _historyTabToggle.onValueChanged.AddListener(OnHistoryToggleHandler);
        EventsAgregator.Subscribe<OnStartGenerateCharsEvent>(OnStartGenerateCharsHandler);
    }

    private void Start()
    {
        SetStartCanvas();
    }

    private void SetStartCanvas()
    {
        _addPlayersTabToggle.isOn = true;
        _addPlayerCanvas.enabled = true;
        _playersTabToggle.isOn = false;
        _playersCanvas.enabled = false;
        _battleTabToggle.isOn = false;
        _battleCanvas.enabled = false;
        _historyTabToggle.isOn = false;
        _historyCanvas.enabled = false;
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

    private void OnHistoryToggleHandler(bool value)
    {
        _historyCanvas.enabled = value;
        if (value)
        {
            var historyController = _historyCanvas.GetComponentInChildren<BattleHistoryController>();
            historyController.LoadTournamentsList();
        }
    }

    public static void SwitchTab(Tab tab)
    {
        switch (tab)
        {
            case Tab.Register:
                _innerInstance._addPlayersTabToggle.isOn = true;
                break;
            case Tab.Players:
                _innerInstance._playersTabToggle.isOn = true;
                break;
            case Tab.Battle:
                _innerInstance._battleTabToggle.isOn = true;
                break;
            case Tab.History:
                _innerInstance._historyTabToggle.isOn = true;
                break;
        }        
    }




}
