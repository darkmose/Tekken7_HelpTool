using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoftEvents;

public static class PlayersHandler
{
    private static Stack<Player> _losePlayers = new Stack<Player>();
    private static List<Player> _playerDiscriptors = new List<Player>();
    public static Stack<Player> LosePlayers => _losePlayers;
    public static List<Player> Players => _playerDiscriptors;
    public static int PlayersCount => _playerDiscriptors.Count;

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
        _losePlayers.Push(player);        
    }
}
