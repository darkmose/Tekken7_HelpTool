using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CharactersCollection
{
    private static List<Character> _characters = new List<Character>();
    public static int CharactersCount => _characters.Count;
    public static List<Character> Characters => _characters;
    public static void PrepareCharacterCollection() 
    {
        var sprites = Resources.LoadAll<Sprite>("Sprites");
        int index = 0;
        foreach (var sprite in sprites)
        {
            _characters.Add(new Character(index++, sprite));
        }
    }

    public static void RemainCharactersInfo() 
    {
        Debug.Log("Remain chars:\n");
        for (int i = 0; i < _characters.Count; i++)
        {
            Debug.Log($"{i+1}. {_characters[i].Name}\n");
        }
    }

    public static Character TakeRandomChar()
    {
        var index = Random.Range(0, CharactersCount);
        var character = Characters[index];
        Characters.RemoveAt(index);
        Debug.Log($"Characters left: {CharactersCount}");
        Debug.Log($"Result character name: {character.Name}");

        return character;
    }

    public static Character[] TakeRemainingCharacters() 
    {
        for (int i = 0; i < _characters.Count; i++)
        {
            _characters[i].Index = i;
        }
        return _characters.ToArray();
    }

}
