using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    private void Awake()
    {
        CharactersCollection.PrepareCharacterCollection();
    }
    void Start()
    {
        
    }

}
