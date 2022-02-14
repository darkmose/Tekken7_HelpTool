using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoftEvents;

public static class PlayersHandler
{
    private static Stack<Player> _losePlayers = new Stack<Player>();
    public static Stack<Player> LosePlayers => _losePlayers;
    private static List<Player> _playerDiscriptors = _playerDiscriptors = new List<Player>();
    public static List<Player> Players => _playerDiscriptors;
    public static int PlayersCount => Players.Count;
    public static Player GetLastPlayer() 
    {
        return Players[Players.Count - 1];
    }    
    public static Player GetFirstPlayer() 
    {
        if (Players.Count > 0)
        {
            return Players[0];
        }
        else
        {
            return null;
        }
    }

    public static Player GetPlayerOfIndex(int index)
    {
        if (Players.Count > 0)
        {
            return Players[index];
        }
        else
        {
            return null;
        }
    }

    public static bool CheckAllPlayersWithCharacters() 
    {
        bool result = false;
        foreach (var player in Players)
        {
            result = player.CharactersCount == PlayersProfiles.CharactersPerPlayer;
            if (!result)
            {
                break;
            }
        }

        return result;
    }

    public static void PlayerOutOfGame(Player player) 
    {
        if (_playerDiscriptors.Contains(player))
        {
            _losePlayers.Push(player);
            _playerDiscriptors.Remove(player);
            for (int i = 0; i < _playerDiscriptors.Count; i++)
            {
                _playerDiscriptors[i].Index = i;
            }
            Debug.Log($"{player.Name} is out of game");
        }
    }

    public static void BackLosePlayerInList() 
    {
        _playerDiscriptors.AddRange(_losePlayers);
        _losePlayers.Clear();
        for (int i = 0; i < _playerDiscriptors.Count; i++)
        {
            _playerDiscriptors[i].Index = i;
        }
    }

}
