using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character
{
    public string Name => CharacterSprite?.name;
    public int Index { get; set; }
    public Sprite CharacterSprite { get; set; }
    public bool IsDroppedOut { get; set; }
    public int WinCount { get; set; }
    public int PerfectCount { get; set; }

    public Character(int index, Sprite characterSprite)
    {
        Index = index;
        CharacterSprite = characterSprite;
    }
}
