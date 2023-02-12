using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoftEvents;

public static class PlayersHandler
{
    private static Stack<Player> _losePlayers = new Stack<Player>();
    public static Stack<Player> LosePlayers => _losePlayers;
    private static List<Player> _playerDiscriptors = new List<Player>();
    public static List<Player> Players => _playerDiscriptors;
    public static int PlayersCount => _playerDiscriptors.Count;

    private static List<Player> _playersInGame = new List<Player>();
    public static List<Player> PlayersInGame => _playersInGame;
    public static int PlayersInGameCount => _playersInGame.Count;
 
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

    public static Player GetFirstPlayerInGame()
    {
        if (PlayersInGameCount > 0)
        {
            return PlayersInGame[0];
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

    public static Player GetPlayerInGameOfIndex(int index)
    {
        if (PlayersInGameCount > 0)
        {
            return PlayersInGame[index];
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

    public static bool CheckAllPlayersInGameWithCharacters()
    {
        bool result = false;
        foreach (var player in PlayersInGame)
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
        if (_playersInGame.Contains(player))
        {
            _losePlayers.Push(player);
            _playersInGame.Remove(player);
            for (int i = 0; i < _playersInGame.Count; i++)
            {
                _playersInGame[i].Index = i;
            }
            Debug.Log($"{player.Name} is out of game");
        }
    }
}
