using UnityEngine;

[CreateAssetMenu(fileName = "Card Data", menuName = "ScriptableObject/Card Data")]
public class CardData : ScriptableObject
{
    public string cardName;
    public string description;

    public Texture2D texture;

    public GameObject prefab;
}
