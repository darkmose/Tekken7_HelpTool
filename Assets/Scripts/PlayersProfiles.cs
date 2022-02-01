using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEvents;
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
    public static int CharactersPerPlayer => innerInstance._charactersCount / PlayerRegister.PlayersRegistered;
    private void Awake()
    {
        if (innerInstance == null)
        {
            innerInstance = this;
        }
        _currentPlayer = new Player("Unknown", -1);
        EventsAgregator.Subscribe<OnPlayerWasAddedEvent>(OnPlayerWasAddedHandler);
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

    private void NextCharacter()
    {
        if (_currentPlayer != null)
        {
            _currentPlayer.NextCharacter();
            _characterCheckoutImage.sprite = _currentPlayer.CurrentCharacter.CharacterSprite;
        }
    }
    private void PrevCharacter()
    {
        if (_currentPlayer != null)
        {
            _currentPlayer.PrevCharacter();
            _characterCheckoutImage.sprite = _currentPlayer.CurrentCharacter.CharacterSprite;
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
        NextCharacter();
    }

    private void OnUpArrowButtonClickHandler()
    {
        PrevCharacter();
    }

    private void PrepareCharactersCount()
    {
        _charactersCount = CharactersCollection.CharactersCount;
    }

    private void RefreshStats() 
    {
        _playerNameText.text = _currentPlayer.Name;
        _winCountText.text = $"Побед: {_currentPlayer.WinCount}";
        _loseCountText.text = $"Поражений: {_currentPlayer.LoseCount}";
        _winRateText.text = $"WinRate: {_currentPlayer.WinRate} %";
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
            var character = _currentPlayer.GetCharacterOfIndex(i);
            var element = ObjectPooler.GetPooledGameObject(PooledElementObjectName);
            element.transform.SetParent(_contentRoot);
            element.transform.localScale = Vector3.one;
            element.name = character.Name;

            if (element.TryGetComponent(out CharsElementHandler charsElementHandler))
            {
                RefreshElement(character, charsElementHandler);
            }
        }


    }

    private void OnGenerateCharactersButtonClickHandler()
    {
        if (PlayerRegister.PlayersRegistered > 0)
        {
            for (int i = 0; i < CharactersPerPlayer; i++)
            {
                var character = CharactersCollection.TakeRandomChar();
                _currentPlayer.AddCharacter(character);
            }
            _characterCheckoutPanel.SetActive(true);
            _characterCheckoutImage.sprite = _currentPlayer.GetCharacterOfIndex(0).CharacterSprite;
            RefreshScrollview();
            _generateCharactersButton.gameObject.SetActive(false);

            EventsAgregator.Post<OnStartGenerateCharsEvent>(this, _onStartGenerateChars);
        }
    }

    private static void RefreshElement(Character character, CharsElementHandler charsElementHandler)
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
