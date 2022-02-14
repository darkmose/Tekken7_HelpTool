using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoftEvents;
using System;
using UnityEngine.UI;

public class PlayersProfiles : MonoBehaviour
{
    private const string PooledElementObjectName = "Element";
    private static PlayersProfiles innerInstance;
    [SerializeField] private Text _playerNameText;
    [SerializeField] private Text _winCountText;
    [SerializeField] private Text _loseCountText;
    [SerializeField] private Text _winRateText;
    [SerializeField] private Button _leftArrowButton;
    [SerializeField] private Button _rightArrowButton;
    [SerializeField] private Button _generateCharactersButton;
    [SerializeField] private Transform _contentRoot;
    [SerializeField] private Transform _pooledObjectsRoot;
    [SerializeField] private GameObject _characterCheckoutPanel;
    [SerializeField] private Button _upArrowButton;
    [SerializeField] private Button _downArrowButton;
    [SerializeField] private Image _characterCheckoutImage;

    private int _charactersCount = 0;
    private Player _currentPlayer;
    private OnStartGenerateCharsEvent _onStartGenerateChars = new OnStartGenerateCharsEvent();
    private Character[] _remainingCharacters;
    private Character _currentRemainCharacter;
    public static int CharactersPerPlayer => innerInstance._charactersCount / PlayerRegister.PlayersRegistered;
    private void Awake()
    {
        if (innerInstance == null)
        {
            innerInstance = this;
        }
        _currentPlayer = new Player("Unknown", -1);
        EventsAgregator.Subscribe<OnPlayerWasAddedEvent>(OnPlayerWasAddedHandler);
        EventsAgregator.Subscribe<OnEndBattleEvent>(EndBattleHandler);
        _leftArrowButton.onClick.AddListener(OnLeftArrowButtonClickHandler);
        _rightArrowButton.onClick.AddListener(OnRightArrowButtonClickHandler);
        _generateCharactersButton.onClick.AddListener(OnGenerateCharactersButtonClickHandler);
        _upArrowButton.onClick.AddListener(OnUpArrowButtonClickHandler);
        _downArrowButton.onClick.AddListener(OnDownArrowButtonClickHandler);
        _generateCharactersButton.gameObject.SetActive(false);
    }


    private void Start()
    {
        ClearScrollView();
        RefreshStats();
        PrepareCharactersCount();
    }

    private void ShowRemainingCharactersPanel()
    {
        CharactersCollection.RemainCharactersInfo();
        _remainingCharacters = CharactersCollection.TakeRemainingCharacters();
        if (_remainingCharacters.Length > 0)
        {
            _currentRemainCharacter = _remainingCharacters[0];
            _characterCheckoutPanel.SetActive(true);
            _characterCheckoutImage.sprite = _currentRemainCharacter.CharacterSprite;
        }
    }

    private void EndBattleHandler(object sender, OnEndBattleEvent data)
    {
        RefreshStats();
        RefreshScrollview();
    }

    private void NextRemainCharacter()
    {
        if (_currentRemainCharacter.Index < _remainingCharacters.Length-1)
        {
            var nextIndex = _currentRemainCharacter.Index + 1;
            _currentRemainCharacter = _remainingCharacters[nextIndex];
            _characterCheckoutImage.sprite = _currentRemainCharacter.CharacterSprite;
        }
        else
        {
            var index = 0;
            _currentRemainCharacter = _remainingCharacters[index];
            _characterCheckoutImage.sprite = _currentRemainCharacter.CharacterSprite;
        }

    }
    private void PrevRemainCharacter()
    {
        if (_currentRemainCharacter.Index > 0)
        {
            var prevIndex = _currentRemainCharacter.Index - 1;
            _currentRemainCharacter = _remainingCharacters[prevIndex];
            _characterCheckoutImage.sprite = _currentRemainCharacter.CharacterSprite;
        }
        else
        {
            var index = _remainingCharacters.Length - 1;
            _currentRemainCharacter = _remainingCharacters[index];
            _characterCheckoutImage.sprite = _currentRemainCharacter.CharacterSprite;
        }

    }

    private void NextPlayer()
    {
        if (_currentPlayer != null)
        {
            if (_currentPlayer.Index < PlayersHandler.PlayersCount - 1)
            {
                int nextIndex = _currentPlayer.Index + 1;
                var player = PlayersHandler.GetPlayerOfIndex(nextIndex);
                _currentPlayer = player;
                RefreshStats();
                RefreshScrollview();

                if (_currentPlayer.CharactersCount == 0)
                {
                    _generateCharactersButton.gameObject.SetActive(true);
                }
                else
                {
                    _generateCharactersButton.gameObject.SetActive(false);
                }
            }
            else
            {
                int nextIndex = 0;
                var player = PlayersHandler.GetPlayerOfIndex(nextIndex);
                _currentPlayer = player;
                RefreshStats();
                RefreshScrollview();

                if (_currentPlayer.CharactersCount == 0)
                {
                    _generateCharactersButton.gameObject.SetActive(true);
                }
                else
                {
                    _generateCharactersButton.gameObject.SetActive(false);
                }
            }
        }
    }

    private void PrevPlayer()
    {
        if (_currentPlayer != null)
        {
            if (_currentPlayer.Index > 0)
            {
                int prevIndex = _currentPlayer.Index - 1;
                var player = PlayersHandler.GetPlayerOfIndex(prevIndex);
                _currentPlayer = player;
                RefreshStats();
                RefreshScrollview();

                if (_currentPlayer.CharactersCount == 0)
                {
                    _generateCharactersButton.gameObject.SetActive(true);
                }
                else
                {
                    _generateCharactersButton.gameObject.SetActive(false);
                }
            }
            else
            {
                int prevIndex = PlayersHandler.PlayersCount - 1;
                var player = PlayersHandler.GetPlayerOfIndex(prevIndex);
                _currentPlayer = player;
                RefreshStats();
                RefreshScrollview();

                if (_currentPlayer.CharactersCount == 0)
                {
                    _generateCharactersButton.gameObject.SetActive(true);
                }
                else
                {
                    _generateCharactersButton.gameObject.SetActive(false);
                }
            }
        }
    }

    private void OnLeftArrowButtonClickHandler()
    {
        PrevPlayer();
    }

    private void OnRightArrowButtonClickHandler()
    {
        NextPlayer();
    }

    private void OnDownArrowButtonClickHandler()
    {
        NextRemainCharacter();
    }

    private void OnUpArrowButtonClickHandler()
    {
        PrevRemainCharacter();
    }

    private void PrepareCharactersCount()
    {
        _charactersCount = CharactersCollection.CharactersCount;
    }

    private void RefreshStats() 
    {
        _playerNameText.text = _currentPlayer.Name;
        _winCountText.text = $"Win: {_currentPlayer.WinCount}";
        _loseCountText.text = $"Lose: {_currentPlayer.LoseCount}";
        _winRateText.text = $"WinRate: {(_currentPlayer.WinRate * 100f).ToString("0.0")} %";
    }

    private void ClearScrollView() 
    {
        var childCount = _contentRoot.childCount;
        for (int i = 0; i < childCount; i++)
        {
            var child = _contentRoot.GetChild(0);
            child.SetParent(_pooledObjectsRoot);
            child.gameObject.SetActive(false);
        }
    }

    private void RefreshScrollview() 
    {
        ClearScrollView();

        for (int i = 0; i < _currentPlayer.CharactersCount; i++)
        {
            _contentRoot.localPosition = Vector3.zero;
            var character = _currentPlayer.GetCharacterOfIndex(i);
            var element = ObjectPooler.GetPooledGameObject(PooledElementObjectName);
            element.transform.SetParent(_contentRoot);
            element.transform.localScale = Vector3.one;
            element.name = character.Name;

            if (element.TryGetComponent(out CharacterUIElement charsElementHandler))
            {
                RefreshUIElement(character, charsElementHandler);
            }
        }
    }

    private void MakeWelcomeSound() 
    {
        AudioManager.PlaySound("WelcomeSound");
    }

    private void OnGenerateCharactersButtonClickHandler()
    {
        if (PlayerRegister.PlayersRegistered > 0)
        {
            MakeWelcomeSound();
            for (int i = 0; i < CharactersPerPlayer; i++)
            {
                var character = CharactersCollection.TakeRandomChar();
                _currentPlayer.AddCharacter(character);
            }
            RefreshScrollview();
            _generateCharactersButton.gameObject.SetActive(false);

            EventsAgregator.Post<OnStartGenerateCharsEvent>(this, _onStartGenerateChars);

            if (PlayersHandler.CheckAllPlayersWithCharacters()) 
            {
                ShowRemainingCharactersPanel();
            }
        }
    }

    private void RefreshUIElement(Character character, CharacterUIElement characterUIElement)
    {
        characterUIElement.SetIndex(character.Index + 1)
            .SetCharImage(character.CharacterSprite)
            .SetDead(character.IsDroppedOut)
            .ClearWinCount()
            .AddPerfectWin(character.PerfectCount)
            .AddWin(character.WinCount - character.PerfectCount); //Кол-во побед включает в себя и победы Perfect
    }

    private void OnPlayerWasAddedHandler(object sender, OnPlayerWasAddedEvent data)
    {
        var player = PlayersHandler.GetFirstPlayer();
        _currentPlayer = player;
        RefreshStats();

        if (_currentPlayer.CharactersCount == 0)
        {
            _generateCharactersButton.gameObject.SetActive(true);
        }
        else
        {
            _generateCharactersButton.gameObject.SetActive(false);
        }
    }
}
