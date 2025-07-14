using System.Collections.Generic;
using UnityEngine;
using Unity.Multiplayer.Playmode;

public class PlayerDataHolder : MonoBehaviour
{
    public static PlayerDataHolder instance;

    [SerializeField] private string name;

    [SerializeField] private List<CardData> deck;


    [SerializeField] CardDatabase cardDatabase;

    public PlayerData playerData { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

#if UNITY_EDITOR
        name = CurrentPlayer.ReadOnlyTags()[0];
#endif

        playerData = new PlayerData(name, cardDatabase.GetDeckIds(deck));
    }
}
