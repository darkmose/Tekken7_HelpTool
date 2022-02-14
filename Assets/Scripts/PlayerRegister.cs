using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SoftEvents;

public class PlayerRegister : MonoBehaviour
{
    private const int MinimumPlayersCount = 2;
    private static PlayerRegister innerInstance;
    [SerializeField] private InputField _nameInputField;
    [SerializeField] private Button _registerButton;
    [SerializeField] private Text _maxPlayerText;
    [SerializeField] private int _maxPlayers;

    public static int PlayersRegistered => innerInstance._registeredPlayers;

    private int _registeredPlayers = 0;
    private OnPlayerWasAddedEvent _playerWasAddedEvent = new OnPlayerWasAddedEvent();

    private void Awake()
    {
        if (innerInstance == null)
        {
            innerInstance = this;
        }
        _registerButton.onClick.AddListener(OnRegisterButtonClick);
    }

    private void Start()
    {
        RefreshDefaultName();
        RefreshMaxPlayers();
    }

    private void RefreshMaxPlayers() 
    {
        string colorRichText = (_registeredPlayers > MinimumPlayersCount) ? "green" : "red";
        _maxPlayerText.text = $"Зарегистрировано игроков:\n<color={colorRichText}>{_registeredPlayers}</color>/{_maxPlayers}";

        if (_registeredPlayers > MinimumPlayersCount)
        {
            TabsSwitcher.SetPlayersTabInteractable(true);
            TabsSwitcher.SetBattleTabInteractable(true);
        }
        else
        {
            TabsSwitcher.SetPlayersTabInteractable(false);
            TabsSwitcher.SetBattleTabInteractable(false);
        }
    }

    private void Register()
    {
        string name = _nameInputField.text;
        int index = _registeredPlayers++;
        var player = new Player(name, index);
        PlayersHandler.Players.Add(player);
        EventsAgregator.Post<OnPlayerWasAddedEvent>(this, _playerWasAddedEvent);
    }

    private void OnRegisterButtonClick()
    {
        if (_registeredPlayers < _maxPlayers)
        {
            Register();
        }
        if (_registeredPlayers == _maxPlayers)
        {
            _registerButton.enabled = false;
        }
        RefreshMaxPlayers();
        RefreshDefaultName();
    }

    private void RefreshDefaultName()
    {
        _nameInputField.text = $"Игрок {_registeredPlayers + 1}";
    }
}
