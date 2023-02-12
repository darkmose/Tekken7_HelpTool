using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SoftEvents;
using System.Text;

public class BattleHandler : MonoBehaviour
{
    private const int MinimumPlayersCount = 2;
    [SerializeField] private Button _firstPlayerWin;
    [SerializeField] private Button _secondPlayerWin;
    [SerializeField] private Text _firstPlayerName;
    [SerializeField] private Text _secondPlayerName;
    [SerializeField] private CharacterView _firstPlayerElement;
    [SerializeField] private CharacterView _secondPlayerElement;
    [SerializeField] private Button _startBattleButton;
    [SerializeField] private GameObject _battlePanel;
    [SerializeField] private Button _firstPerfectButton;
    [SerializeField] private Button _secondPerfectButton;
    [SerializeField] private Button _drawWinButton;
    [SerializeField] private Text _leaderBoardText;
    [SerializeField] private GameObject _leaderBoardPanel;
    [SerializeField] private Button _leaderBoardCloseButton;
    private List<Player> _playersInGame = new List<Player>();
    private Player _firstPlayer;
    private Player _secondPlayer;
    private OnEndBattleEvent _onEndBattleEvent = new OnEndBattleEvent();
    private OnEndTournamentEvent _onEndTournamentEvent = new OnEndTournamentEvent();
    private int _battleIndex = 0;
    private bool _isFirstCircle = true;
    private int ActivePlayersCount => _playersInGame.Count;

    private void Awake()
    {
        _firstPlayerWin.onClick.AddListener(OnFirstPlayerWinHandler);
        _secondPlayerWin.onClick.AddListener(OnSecondPlayerWinHandler);
        _startBattleButton.onClick.AddListener(OnStartBattleClickHandler);
        _drawWinButton.onClick.AddListener(OnDrawWinButton);
        _firstPerfectButton.onClick.AddListener(OnFirstPerfectButtonClickHandler);
        _secondPerfectButton.onClick.AddListener(OnSecondPerfectButtonClickHandler);
        _leaderBoardCloseButton.onClick.AddListener(OnLeaderboardCloseButtonClickHandler);
        EventsAgregator.Subscribe<OnEndBattleEvent>(EndBattleHandler);
    }

    private void PreparePlayersInGame()
    {
        _playersInGame.Clear();

        foreach (var player in PlayersHandler.Players)
        {
            _playersInGame.Add(player);
        }
    }

    private void OnStartBattleClickHandler()
    {
        if (PlayersHandler.CheckAllPlayersWithCharacters())
        {
            PreparePlayersInGame();
            _battlePanel.SetActive(true);
            PrepareOneByOnePlayers();
            PrepareCharsToFight();
            RefreshBattleWindow();
            _startBattleButton.gameObject.SetActive(false);
        }
        else
        {
            TabsSwitcher.SwitchTab(TabsSwitcher.Tab.Players);
        }
    }

    private void OnLeaderboardCloseButtonClickHandler()
    {
        TabsSwitcher.SetBattleTabInteractable(false);
        _leaderBoardPanel.SetActive(false);
        TabsSwitcher.SwitchTab(TabsSwitcher.Tab.Players);
    }

    private void EndBattleHandler(object sender, OnEndBattleEvent data)
    {
        _battlePanel.SetActive(false);
        CheckTournamentEnd();
        NextBattle();
    }

    private void ShowLeaderBoard(Player winner)
    {
        var result = new StringBuilder();
        result.Append("1. " + winner.Name + "\n");
        int i = 2;
        foreach (var player in PlayersHandler.LosePlayers)
        {
            result.Append((i++).ToString() + ". " + player.Name + "\n");
        }

        _leaderBoardText.text = result.ToString();
        _leaderBoardPanel.SetActive(true);
    }

    private void OnFirstPlayerWinHandler()
    {
        _firstPlayer.Win(false);
        if (_secondPlayer.LoseAndCheckGameOver())
        {
            _playersInGame.Remove(_secondPlayer);
        }
        EventsAgregator.Post<OnEndBattleEvent>(this, _onEndBattleEvent);
    }

    private void OnSecondPlayerWinHandler()
    {
        _secondPlayer.Win(false);
        if (_firstPlayer.LoseAndCheckGameOver())
        {
            _playersInGame.Remove(_firstPlayer);
        }
        EventsAgregator.Post<OnEndBattleEvent>(this, _onEndBattleEvent);
    }

    private void OnSecondPerfectButtonClickHandler()
    {
        _secondPlayer.Win(true);
        if (_firstPlayer.LoseAndCheckGameOver())
        {
            _playersInGame.Remove(_firstPlayer);
        }
        EventsAgregator.Post<OnEndBattleEvent>(this, _onEndBattleEvent);
    }

    private void OnFirstPerfectButtonClickHandler()
    {
        _firstPlayer.Win(true);
        if (_secondPlayer.LoseAndCheckGameOver())
        {
            _playersInGame.Remove(_secondPlayer);
        }
        EventsAgregator.Post<OnEndBattleEvent>(this, _onEndBattleEvent);
    }

    private void OnDrawWinButton()
    {
        _firstPlayer.Win(false);
        _secondPlayer.Win(false);
        EventsAgregator.Post<OnEndBattleEvent>(this, _onEndBattleEvent);
    }

    private void NextBattle() 
    {                    
        if (_isFirstCircle)
        {
            PrepareOneByOnePlayers();
            PrepareCharsToFight();
            RefreshBattleWindow();
            _battlePanel.SetActive(true);
        }
        else
        {
            PrepareOneThroughOnePlayers();
            PrepareCharsToFight();
            RefreshBattleWindow();
            _battlePanel.SetActive(true);
        }
    }

    private void PrepareCharsToFight() 
    {
        _firstPlayer.RandomSetCurrentCharacter();
        _secondPlayer.RandomSetCurrentCharacter();
    }

    private void RefreshBattleWindow()
    {
        _firstPlayerName.text = _firstPlayer.Name;
        _secondPlayerName.text = _secondPlayer.Name;
        RefreshUIElement(_firstPlayerElement, _firstPlayer.CurrentCharacter);
        RefreshUIElement(_secondPlayerElement, _secondPlayer.CurrentCharacter);
    }

    private void RefreshUIElement(CharacterView characterUIElement, Character character) 
    {
        characterUIElement.SetIndex(character.Index + 1)
            .SetCharImage(character.CharacterSprite)
            .SetDead(character.IsDroppedOut)
            .SetPerfectWinCount(character.PerfectCount)
            .SetWinCount(character.WinCount - character.PerfectCount); //Кол-во побед включает в себя и победы Perfect
    }

    private void PrepareOneByOnePlayers() 
    {
        if (_battleIndex >= ActivePlayersCount)
        {
            _battleIndex = 0;
            _isFirstCircle = false;
            PrepareOneThroughOnePlayers();
            return;
        }
        _firstPlayer = _playersInGame[_battleIndex];
        if (_battleIndex < ActivePlayersCount - 1)
        {
            _secondPlayer = _playersInGame[_battleIndex + 1];
            _battleIndex++;
        }
        else
        {
            _secondPlayer = _playersInGame[0];
            _isFirstCircle = false;
            _battleIndex = 0;
        }        
    }

    private void PrepareOneThroughOnePlayers() 
    {
        if (ActivePlayersCount > MinimumPlayersCount)
        {
            if (_battleIndex < ActivePlayersCount - 2)
            {
                _firstPlayer = _playersInGame[_battleIndex];
                _secondPlayer = _playersInGame[_firstPlayer.Index + 2];
                _battleIndex++;
                return;
            }
        }

        _battleIndex = 0;
        _isFirstCircle = true;
        PrepareOneByOnePlayers();        
    }

    private void CheckTournamentEnd()
    {
        if (ActivePlayersCount <= 1)
        {
            var winner = _playersInGame[0];
            Debug.Log($"Победитель: {winner.Name}");
            _onEndTournamentEvent.Winner = winner;
            ShowLeaderBoard(winner);
            EventsAgregator.Post<OnEndTournamentEvent>(this, _onEndTournamentEvent);
        }
    }
}
