using Database;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TournamentView : MonoBehaviour
{
    [SerializeField] private GameObject _windowRoot;
    [SerializeField] private Text _playerName;
    [SerializeField] private Text _winCount;
    [SerializeField] private Text _loseCount;
    [SerializeField] private Text _winrate;
    [SerializeField] private Button _prevPlayer;
    [SerializeField] private Button _nextPlayer;
    [SerializeField] private Transform _contentRoot;

    private Action _backCallback;

    public void InitButtonsCallbacks(Action nextPlayerCallback, Action prevPlayerCallback, Action backCallback)
    {
        _prevPlayer.onClick.RemoveAllListeners();
        _nextPlayer.onClick.RemoveAllListeners();
        _prevPlayer.onClick.AddListener(()=> prevPlayerCallback?.Invoke());
        _nextPlayer.onClick.AddListener(() => nextPlayerCallback?.Invoke());
        _backCallback = backCallback;
    }

    public void SetData(PlayerDataDescriptor playerDataDescriptor, List<CharacterDataDescriptor> characterDatas)
    {
        _playerName.text = playerDataDescriptor.PlayerName;
        _winCount.text = playerDataDescriptor.WinCount.ToString();
        _loseCount.text = playerDataDescriptor.LoseCount.ToString();
        _winrate.text = playerDataDescriptor.WinRate.ToString("0.0");
        LoadPlayerCharacters(characterDatas);
    }

    private void LoadPlayerCharacters(List<CharacterDataDescriptor> characterDatas)
    {
        ClearContentRoot();
        foreach (var character in characterDatas)
        {
            var charNumber = character.CharacterID + 1;
            var charWins = character.WinCount;
            var charPerfects = character.PerfectCount;
            var isDead = character.isDead;
            var sprite = Resources.Load<Sprite>($"Sprites/{character.CharacterName}");

            var characterUIElement = ObjectPooler.GetPooledGameObject("Element");
            characterUIElement.transform.SetParent(_contentRoot);
            characterUIElement.transform.localScale = Vector3.one;
            if (characterUIElement.TryGetComponent(out CharacterView characterView))
            {
                characterView.SetDead(isDead)
                .SetIndex(charNumber)
                .SetWinCount(charWins)
                .SetPerfectWinCount(charPerfects)
                .SetCharImage(sprite);
            }
        }
    }

    private void ClearContentRoot()
    {
        var contentCount = _contentRoot.childCount;
        for (int i = 0; i < contentCount; i++)
        {
            var contentElement = _contentRoot.GetChild(0);
            contentElement.gameObject.SetActive(false);
            contentElement.SetParent(ObjectPooler.PooledObjectsRoot);
        }
    }

    public void Show()
    {
        _windowRoot.SetActive(true);
    }

    public void Hide()
    {
        _windowRoot.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_windowRoot.activeSelf)
            {
                _backCallback?.Invoke();
            }
        }
    }
}