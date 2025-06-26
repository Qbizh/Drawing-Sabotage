using UnityEngine;

public class DeckWrapper : MonoBehaviour
{
    public void OnCardChange()
    {
        GetComponentInParent<CardBoard>().LoadCardData();
    }
}
