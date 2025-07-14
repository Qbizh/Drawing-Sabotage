using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Card Database", menuName = "ScriptableObject/Card Database")]
public class CardDatabase : ScriptableObject
{
    [SerializeField] List<CardData> cardDatabase;

    public CardData GetCard(int id)
    {
        return cardDatabase[id];
    }

    public List<CardData> GetDeck(int[] ids)
    {
        List<CardData> deck = new List<CardData>();

        foreach (int id in ids) 
        {
            deck.Add(GetCard(id));
        }

        return deck;
    }

    public int GetCardId(CardData data)
    {
        return cardDatabase.FindIndex(c => c.cardName == data.cardName);
    }

    public int[] GetDeckIds(List<CardData> deck) 
    { 
        int[] ids = new int[deck.Count];

        for (int i = 0; i < deck.Count; i++)
        {
            ids[i] = GetCardId(deck[i]);
        }

        return ids;
    }
}
