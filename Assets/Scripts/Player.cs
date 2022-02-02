using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private const int MaximumWaveCount = 25;
    private List<Character> _charactersList = new List<Character>();
    public int CharactersCount => _charactersList.Count;
    public int TournamentWave { get; private set; }
    public string Name { get; set; }
    public int Index { get; set; }
    public int WinCount { get; private set; }
    public int LoseCount { get; private set; }
    private int GamesPlayed => WinCount + LoseCount;
    public float WinRate
    {
        get
        {
            if (GamesPlayed != 0)
            {
                return (float)WinCount / (float)GamesPlayed;
            }
            else
            {
                return 0;
            }
        }
    }
    private int LastIndex => _charactersList.Count - 1;
    public Character CurrentCharacter { get; private set; }
    public bool IsLoseGame;
    public Player(string name, int index) 
    {
        Name = name;
        Index = index;
    }

    public void Win() 
    {
        WinCount++;
    }

    public void Lose() 
    {
        LoseCount++;
    }

    public void AddCharacter(Character character) 
    {
        _charactersList.Add(character);
        character.Index = LastIndex;
        if (LastIndex == 0)
        {
            CurrentCharacter = character;
        }
    }
    public Character GetCharacterOfIndex(int index) 
    {
        return _charactersList.Find(data => data.Index == index);
    }
    
    private List<Character> ValidCharactersList { get; set; }

    public void RandomSetCurrentCharacter()
    {
        var wave = 1;
        ValidCharactersList = _charactersList.FindAll(character => character.WinCount <= wave && !character.IsDroppedOut);
        if (ValidCharactersList.Count != 0)
        {
            var randVal = Random.Range(0, _charactersList.Count);
            CurrentCharacter = _charactersList[randVal];
        }
        else
        {

            for (int i = CurrentCharacter.Index + 1; i < CharactersCount; i++)
            {

                if (!_charactersList[i].IsDroppedOut)
                {
                    CurrentCharacter = _charactersList[i];
                    break;
                }
                if (i == CharactersCount - 1 && !IsLoseGame)
                {
                    i = 0;
                }
            }
            
        }
    }

    public void CheckForTournamentWave() 
    {
        if (IsLoseGame)
        {
            return;
        }
        for (int i = 0; i < MaximumWaveCount; i++)
        {
            var chars = _charactersList.FindAll(charr => !charr.IsDroppedOut && charr.WinCount == i);
            if (chars.Count != 0)
            {
                TournamentWave = i;
            }
        }
        TournamentWave = 1;
    }

    public void CheckForLoseGame() 
    {
        var chars = _charactersList.FindAll(data=>!data.IsDroppedOut);
        if (chars.Count == 0)
        {
            IsLoseGame = true;
        }
    }

    public void NextCharacter() 
    {
        if (CurrentCharacter.Index < CharactersCount - 1)
        {
            CurrentCharacter = _charactersList[ CurrentCharacter.Index + 1];
        }
    }

    public void PrevCharacter() 
    {
        if (CurrentCharacter.Index > 0)
        {
            CurrentCharacter = _charactersList[ CurrentCharacter.Index - 1];
        }
    }
}
