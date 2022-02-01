using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayersHandler
{
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

}
