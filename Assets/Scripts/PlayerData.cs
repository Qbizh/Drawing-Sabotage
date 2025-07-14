using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public string name;

    public int[] deck;

    public int index;

    public PlayerData(string newName, int[] newDeck)
    {
        name = newName;
        deck = newDeck;
    }

    public PlayerData() {}
}
