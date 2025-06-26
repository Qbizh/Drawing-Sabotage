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

    public int GetCardId(CardData data)
    {
        return cardDatabase.FindIndex(c => c.cardName == data.cardName);
    }
}
