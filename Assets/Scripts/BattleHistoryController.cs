using Database;
using SoftEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleHistoryController : MonoBehaviour
{
    private const int START_RANGE_INDEX = 0;
    private const int QUERY_RANGE_COUNT = 10;

    [SerializeField] private BattleHistorySQLiteManager _battleHistorySQLiteManager;
    [SerializeField] private TournamentsListView _tournamentsListView;
    [SerializeField] private TournamentView _tournamentView;
    private int _currentStartIndex = START_RANGE_INDEX;
    private int _currentPlayerID = 0;
    private int _currentTournamentID = 0;
    private List<PlayerDataDescriptor> _playerDatas;
    private List<TournamentDataDescriptor> _lastTournamentsRange;

    private void Awake()
    {
        InitTournamentListViewCallbacks();
        InitTournamentViewCallbacks();
        EventsAgregator.Subscribe<OnEndTournamentEvent>(OnTournamentEndHandler);
    }

    private void OnTournamentEndHandler(object sender, OnEndTournamentEvent data)
    {
        var tournamentData = new TournamentDataDescriptor();
        tournamentData.Date = DateTime.Now.Date.ToShortTimeString();
        tournamentData.IsTournamentOver = true;
        var tournamentID = _battleHistorySQLiteManager.GetLastTournamentIndex() + 1;
        tournamentData.TournamentID = tournamentID;
        tournamentData.Winner = data.Winner.Name;

        tournamentData.PlayerDatas = new List<PlayerDataDescriptor>();
        foreach (var player in PlayersHandler.Players)
        {
            var playerData = new PlayerDataDescriptor();
            playerData.PlayerID = player.Index;
            playerData.PlayerName = player.Name;
            playerData.WinCount = player.WinCount;
            playerData.LoseCount = player.LoseCount;
            playerData.WinRate = player.WinRate;
            playerData.CharactersDatas = new List<CharacterDataDescriptor>();

            for (int i = 0; i < player.CharactersCount; i++)
            {
                var character = player.GetCharacterOfIndex(i);
                var characterData = new CharacterDataDescriptor();
                characterData.CharacterID = character.Index;
                characterData.CharacterName = character.Name;
                characterData.isDead = character.IsDroppedOut;
                characterData.WinCount = character.WinCount;
                characterData.PerfectCount = character.PerfectCount;
                playerData.CharactersDatas.Add(characterData);
            }

            tournamentData.PlayerDatas.Add(playerData);
        }

        SaveTournament(tournamentData);
    }

    private void InitTournamentViewCallbacks()
    {
        _tournamentView.InitButtonsCallbacks(OnNextPlayer, OnPrevPlayer, OnTournamentViewClose);
    }

    private void OnTournamentViewClose()
    {
        _tournamentView.Hide();
    }

    private void OnPrevPlayer()
    {
        _currentPlayerID--;
        if (_currentPlayerID < 0)
        {
            _currentPlayerID = _playerDatas.Count - 1;
        }
        LoadPlayerCharacters(_currentPlayerID, _currentTournamentID);
    }

    private void OnNextPlayer()
    {
        _currentPlayerID++;
        if (_currentPlayerID >= _playerDatas.Count)
        {
            _currentPlayerID = 0;
        }
        LoadPlayerCharacters(_currentPlayerID, _currentTournamentID);
    }

    private void InitTournamentListViewCallbacks()
    {
        _tournamentsListView.InitButtonsCallbacks(OnNextTournamentsList, OnPrevTournamentsList, OnTournamentElementClick);
    }

    private void OnTournamentElementClick(int tournamentID)
    {
        _tournamentsListView.Hide();
        _currentTournamentID = tournamentID;
        var tournament = _lastTournamentsRange.Find(tournament => tournament.TournamentID == tournamentID);
        _playerDatas = _battleHistorySQLiteManager.LoadPlayersFromTournament(tournamentID);
        LoadPlayerCharacters(_currentPlayerID, tournamentID);
        _tournamentView.Show();
    }

    private void LoadPlayerCharacters(int playerIndex, int tournamentID)
    {
        var player = _playerDatas[playerIndex];
        var playerCharacters = _battleHistorySQLiteManager.LoadCharactersOfPlayerFromTournament(tournamentID, player.PlayerID);
        _tournamentView.SetData(player, playerCharacters);
    }

    private void OnPrevTournamentsList()
    {
        int startID = _currentStartIndex - QUERY_RANGE_COUNT;
        int lastID = startID + QUERY_RANGE_COUNT - 1;
        _currentStartIndex = startID;
        LoadTournamentsRange(startID, lastID);        
    }

    private void OnNextTournamentsList()
    {
        int startID = _currentStartIndex + QUERY_RANGE_COUNT;
        int lastID = startID + QUERY_RANGE_COUNT - 1;
        _currentStartIndex = startID;
        LoadTournamentsRange(startID, lastID);
    }

    private void SaveTournament(TournamentDataDescriptor tournamentDataDescriptor)
    {
        var tournamentID = tournamentDataDescriptor.TournamentID.ToString();
        var date = tournamentDataDescriptor.Date;
        var isOver = tournamentDataDescriptor.IsTournamentOver ? "1" : "0";
        var winner = tournamentDataDescriptor.Winner;
        _battleHistorySQLiteManager.InsertTournament(tournamentID, date, isOver, winner);

        foreach (var playerData in tournamentDataDescriptor.PlayerDatas)
        {
            var playerID = playerData.PlayerID.ToString();
            var loseCount = playerData.LoseCount.ToString();
            var winCount = playerData.WinCount.ToString();
            var winRate = playerData.WinRate.ToString();
            var playerName = playerData.PlayerName;
            _battleHistorySQLiteManager.InsertPlayer(tournamentID, playerID, winCount, loseCount, winRate, playerName);

            foreach (var character in playerData.CharactersDatas)
            {
                var characterID = character.CharacterID.ToString();
                var charWinCount = character.WinCount.ToString();
                var charWinPerfects = character.PerfectCount.ToString();
                var isDead = character.isDead.ToString();
                var charName = character.CharacterName;
                _battleHistorySQLiteManager.InsertCharacter(tournamentID, playerID, characterID, isDead, charWinCount, charWinPerfects, charName);
            }
        }
    }

    public void LoadTournamentsList()
    {
        _currentStartIndex = START_RANGE_INDEX;
        int startID = START_RANGE_INDEX;
        int lastID = QUERY_RANGE_COUNT - 1;
        LoadTournamentsRange(startID, lastID);
    }

    private void LoadTournamentsRange(int startID, int lastID)
    {
        var tournamentsDatas = _battleHistorySQLiteManager.LoadTournamentsRange(startID, lastID);
        var lastTournamentsIndex = _battleHistorySQLiteManager.GetLastTournamentIndex();
        var canMoveNext = lastID < lastTournamentsIndex;
        var canMovePrev = startID >= QUERY_RANGE_COUNT;
        _tournamentsListView.PrepareList(tournamentsDatas);
        _tournamentsListView.RefreshNavigationPanel(startID, lastID, canMoveNext, canMovePrev);
        _tournamentsListView.Show();
        _lastTournamentsRange = tournamentsDatas;
    }
}
