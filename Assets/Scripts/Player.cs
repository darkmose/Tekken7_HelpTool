using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private const int MaximumWaveCount = 25;
    private List<Character> _charactersList = new List<Character>();
    public int CharactersCount => _charactersList.Count;
    public string Name { get; set; }
    public int Index { get; set; }
    public int WinCount { get; private set; }
    public int LoseCount { get; private set; }
    private List<Character> ValidCharactersList { get; set; }
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
    public Character CurrentCharacter { get; private set; }
    public bool IsLoseGame;

    public Player(string name, int index) 
    {
        Name = name;
        Index = index;
        ValidCharactersList = new List<Character>();
    }

    public void Win(bool isPerfect) 
    {
        WinCount++;
        CurrentCharacter.WinCount++;
        if (isPerfect)
        {
            CurrentCharacter.PerfectCount++;
        }
    }

    public bool LoseAndCheckGameOver() 
    {
        LoseCount++;
        CurrentCharacter.IsDroppedOut = true;
        return CheckForLoseGame();
    }

    public void AddCharacter(Character character, int index) 
    {
        _charactersList.Add(character);
        character.Index = index;
        if (index == 0)
        {
            CurrentCharacter = character;
        }
    }

    public Character GetCharacterOfIndex(int index) 
    {
        return _charactersList.Find(data => data.Index == index);
    }   

    public void RandomSetCurrentCharacter()
    {
        var wave = CheckForTournamentWave();
        ValidCharactersList.Clear();
        ValidCharactersList.AddRange(_charactersList.FindAll(character => character.WinCount <= wave && !character.IsDroppedOut));
        if (ValidCharactersList.Count != 0)
        {
            var randVal = Random.Range(0, ValidCharactersList.Count);
            CurrentCharacter = ValidCharactersList[randVal];
        }
    }

    private int CheckForTournamentWave() 
    {
        for (int i = 0; i < MaximumWaveCount; i++)
        {
            var chars = _charactersList.FindAll(charr => !charr.IsDroppedOut && charr.WinCount == i);
            if (chars.Count != 0)
            {
                return i;
            }
        }
        return 0;
    }

    private bool CheckForLoseGame() 
    {
        var chars = _charactersList.FindAll(data=>!data.IsDroppedOut);
        if (chars.Count == 0)
        {
            IsLoseGame = true;
            PlayersHandler.PlayerOutOfGame(this);
        }
        return IsLoseGame;
    }
}
