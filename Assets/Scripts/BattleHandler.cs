using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameEvents;

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
    [SerializeField] private CharsElementHandler _firstPlayerElement;
    [SerializeField] private CharsElementHandler _secondPlayerElement;
    [SerializeField] private Button _startBattleButton;
    [SerializeField] private Button _nextBattleButton;
    [SerializeField] private GameObject _battlePanel;
    [SerializeField] private Button _firstPerfectButton;
    [SerializeField] private Button _secondPerfectButton;
    [SerializeField] private Button _drawWinButton;
    private Player _firstPlayer;
    private Player _secondPlayer;

    private OnEndBattleEvent _onEndBattleEvent = new OnEndBattleEvent();

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

    private void OnDrawWinButton()
    {
        _firstPlayer.Win();
        _secondPlayer.Win();
        _onEndBattleEvent.winner = _firstPlayer;
        _onEndBattleEvent.loser = _secondPlayer;
        _onEndBattleEvent.isDraw = true;
        EventsAgregator.Post<OnEndBattleEvent>(this, _onEndBattleEvent);
        EndBattle();
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
                PrepareFirstCirclePlayers();
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
        if (isFirstCircle)
        {
            PrepareFirstCirclePlayers();
            PrepareCharsToFight();
            RefreshBattleWindow();
            _battlePanel.SetActive(true);
        }
        else
        {
            PrepareSecondCirclePlayers();
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

    private void RefreshUIElement(CharsElementHandler charsElementHandler, Character character) 
    {
        charsElementHandler.charImage.sprite = character.CharacterSprite;
        charsElementHandler.deadImage.enabled = character.IsDroppedOut;
        charsElementHandler.indexText.text = $"{character.Index + 1}.";

        for (int i = 0; i < character.WinCount; i++)
        {
            var v = ObjectPooler.GetPooledGameObject("V");
            if (i < character.PerfectCount)
            {
                v.transform.localScale = Vector3.one * 1.5f;
            }
            v.transform.SetParent(charsElementHandler.winCountTransform);
        }
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
        EndBattle();
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
        EndBattle();
    }

    private void EndBattle() 
    {
        _nextBattleButton.gameObject.SetActive(true);
        _battlePanel.SetActive(false);
        _firstPlayer.CheckForLoseGame();
        _secondPlayer.CheckForLoseGame();
    }

    private void OnBattleToggleClickHandler(bool value)
    {
        _battleCanvas.enabled = value;
    }

    private void PrepareFirstCirclePlayers() 
    {
        if (PlayersHandler.PlayersCount > MinimumPlayersCount)
        {
            _firstPlayer = PlayersHandler.GetPlayerOfIndex(_battleIndex);
            if (_firstPlayer.Index < PlayerRegister.PlayersRegistered - 1)
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
    }
    private void PrepareSecondCirclePlayers() 
    {
        if (PlayersHandler.PlayersCount > MinimumPlayersCount)
        {
            _firstPlayer = PlayersHandler.GetPlayerOfIndex(_battleIndex);
            if (_firstPlayer.Index < PlayerRegister.PlayersRegistered - 2)
            {
                _secondPlayer = PlayersHandler.GetPlayerOfIndex(_firstPlayer.Index + 2);
                _battleIndex++;
            }
            else
            {
                isFirstCircle = true;
                _battleIndex = 0;

                PrepareFirstCirclePlayers();
            }
        }

    }

}
