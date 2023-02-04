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
    private const int PlayersTabIndex = 1;
    [SerializeField] private Toggle _battleTabButton;
    [SerializeField] private Canvas _battleCanvas;
    [SerializeField] private Button _firstPlayerWin;
    [SerializeField] private Button _secondPlayerWin;
    [SerializeField] private Text _firstPlayerName;
    [SerializeField] private Text _secondPlayerName;
    [SerializeField] private CharacterView _firstPlayerElement;
    [SerializeField] private CharacterView _secondPlayerElement;
    [SerializeField] private Button _startBattleButton;
    [SerializeField] private Button _nextBattleButton;
    [SerializeField] private GameObject _battlePanel;
    [SerializeField] private Button _firstPerfectButton;
    [SerializeField] private Button _secondPerfectButton;
    [SerializeField] private Button _drawWinButton;
    [SerializeField] private Text _leaderBoardText;
    [SerializeField] private GameObject _leaderBoardPanel;
    [SerializeField] private Button _leaderBoardCloseButton;
    private Player _firstPlayer;
    private Player _secondPlayer;

    private OnEndBattleEvent _onEndBattleEvent = new OnEndBattleEvent();
    private OnEndGameEvent _onEndGameEvent = new OnEndGameEvent();

    private int _battleIndex = 0;
    private bool isFirstCircle = true;

    private void Awake()
    {
        _battleTabButton.onValueChanged.AddListener(OnBattleToggleClickHandler);
        _firstPlayerWin.onClick.AddListener(OnFirstPlayerWinHandler);
        _secondPlayerWin.onClick.AddListener(OnSecondPlayerWinHandler);
        _startBattleButton.onClick.AddListener(OnStartBattleClickHandler);
        _nextBattleButton.onClick.AddListener(OnNextBattleButtonClickHandler);
        _drawWinButton.onClick.AddListener(OnDrawWinButton);
        _firstPerfectButton.onClick.AddListener(OnFirstPerfectButtonClickHandler);
        _secondPerfectButton.onClick.AddListener(OnSecondPerfectButtonClickHandler);
        _leaderBoardCloseButton.onClick.AddListener(OnLeaderboardCloseButtonClickHandler);
        EventsAgregator.Subscribe<OnEndBattleEvent>(EndBattleHandler);
    }

    private void OnLeaderboardCloseButtonClickHandler()
    {
        PlayersHandler.BackLosePlayerInList();
        TabsSwitcher.SetBattleTabInteractable(false);
        TabsSwitcher.SwitchTab(1);
        _leaderBoardPanel.SetActive(false);
    }

    private void EndBattleHandler(object sender, OnEndBattleEvent data)
    {
        _battlePanel.SetActive(false);
        data.loser.CheckForLoseGame();
        NextBattle();
    }

    private void OnSecondPerfectButtonClickHandler()
    {
        OnSecondPlayerWinHandler();
        _secondPlayer.CurrentCharacter.PerfectCount++;
    }

    private void OnFirstPerfectButtonClickHandler()
    {
        OnFirstPlayerWinHandler();
        _firstPlayer.CurrentCharacter.PerfectCount++;
    }

    private void OnDrawWinButton() //Draw - ничья
    {
        _firstPlayer.Win();
        _secondPlayer.Win();
        _firstPlayer.CurrentCharacter.WinCount++;
        _secondPlayer.CurrentCharacter.WinCount++;
        _onEndBattleEvent.winner = _firstPlayer;
        _onEndBattleEvent.loser = _secondPlayer;
        _onEndBattleEvent.isDraw = true;
        EventsAgregator.Post<OnEndBattleEvent>(this, _onEndBattleEvent);
    }

    private void Start()
    {
        _nextBattleButton.gameObject.SetActive(false);
    }

    private void OnNextBattleButtonClickHandler()
    {
        NextBattle();
    }

    private void OnStartBattleClickHandler()
    {
        if (PlayersHandler.CheckAllPlayersWithCharacters())
        {
            _battlePanel.SetActive(true);
            if (isFirstCircle)
            {
                PrepareOneByOnePlayers();
            }
            PrepareCharsToFight();
            RefreshBattleWindow();
            _startBattleButton.gameObject.SetActive(false);
        }
        else
        {
            TabsSwitcher.SwitchTab(PlayersTabIndex);
        }
    }

    private void NextBattle() 
    {                    
        if (isFirstCircle && PlayersHandler.PlayersCount > 2)
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
            .ClearWinCount()
            .AddPerfectWin(character.PerfectCount)
            .AddWin(character.WinCount - character.PerfectCount); //Кол-во побед включает в себя и победы Perfect
    }


    private void OnFirstPlayerWinHandler()
    {
        _firstPlayer.CurrentCharacter.WinCount++;
        _firstPlayer.Win();
        _secondPlayer.Lose();
        _secondPlayer.CurrentCharacter.IsDroppedOut = true;
        _onEndBattleEvent.winner = _firstPlayer;
        _onEndBattleEvent.loser = _secondPlayer;
        EventsAgregator.Post<OnEndBattleEvent>(this, _onEndBattleEvent);
    }



    private void OnSecondPlayerWinHandler()
    {
        _secondPlayer.CurrentCharacter.WinCount++;
        _secondPlayer.Win();
        _firstPlayer.Lose();
        _firstPlayer.CurrentCharacter.IsDroppedOut = true;
        _onEndBattleEvent.winner = _secondPlayer;
        _onEndBattleEvent.loser = _firstPlayer;
        EventsAgregator.Post<OnEndBattleEvent>(this, _onEndBattleEvent);
    }


    private void OnBattleToggleClickHandler(bool value)
    {
        _battleCanvas.enabled = value;
    }

    private void PrepareOneByOnePlayers() 
    {
        if (PlayersHandler.PlayersCount > 1)
        {
            _firstPlayer = PlayersHandler.GetPlayerOfIndex(_battleIndex);
            if (_firstPlayer.Index < PlayersHandler.PlayersCount - 1)
            {
                _secondPlayer = PlayersHandler.GetPlayerOfIndex(_firstPlayer.Index + 1);
                _battleIndex++;
            }
            else
            {
                _secondPlayer = PlayersHandler.GetFirstPlayer();
                isFirstCircle = false;
                _battleIndex = 0;
            }
        }
        else
        {
            Debug.Log($"Победитель: {PlayersHandler.GetPlayerOfIndex(0).Name}");
            _onEndGameEvent.Winner = PlayersHandler.GetPlayerOfIndex(0);
            ShowLeaderBoard(_onEndGameEvent.Winner);
            EventsAgregator.Post<OnEndGameEvent>(this, _onEndGameEvent);
        }
    }

    private void ShowLeaderBoard(Player winner) 
    {
        var result = new StringBuilder();
        result.Append("1. " + winner.Name +"\n");
        int i = 2;
        foreach (var player in PlayersHandler.LosePlayers)
        {
            result.Append((i++).ToString() + ". " + player.Name + "\n");
        }

        _leaderBoardText.text = result.ToString();
        _leaderBoardPanel.SetActive(true);
    }

    private void PrepareOneThroughOnePlayers() 
    {
        if (PlayersHandler.PlayersCount > MinimumPlayersCount)
        {
            _firstPlayer = PlayersHandler.GetPlayerOfIndex(_battleIndex);
            if (_firstPlayer.Index < PlayersHandler.PlayersCount - 2)
            {
                _secondPlayer = PlayersHandler.GetPlayerOfIndex(_firstPlayer.Index + 2);
                _battleIndex++;
            }
            else
            {
                isFirstCircle = true;
                _battleIndex = 0;
                PrepareOneByOnePlayers();
            }
        }
        else
        {
            _battleIndex = 0;
            isFirstCircle = true;
            PrepareOneByOnePlayers();
        }

    }

}
