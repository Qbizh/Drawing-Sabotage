using System;
using System.Timers;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using TMPro;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    float roundTime = 15;

    [SerializeField] CardDatabase cardDatabase;

    [SerializeField] TMP_Text timerDisplay;

    public readonly SyncTimer gameTimer;

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();

        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }

    public void InitializeGame()
    {
        InitializeGameClient();

        gameTimer.StartTimer(roundTime);
    }

    [ObserversRpc]
    public void InitializeGameClient()
    {
        CardsManager.instance.Init(cardDatabase.GetDeck(PlayerDataHolder.instance.playerData.deck));

        foreach (var client in InstanceFinder.ClientManager.Clients)
        {
            if (client.Value != InstanceFinder.ClientManager.Connection)
            {
                PipesManager.instance.AddPlayerPipe(client.Value);
            }
        }
    }

    private void Update()
    {
        gameTimer.Update();

        TimeSpan time = TimeSpan.FromSeconds(gameTimer.Remaining);
        string display = time.ToString("m\\:ss");

        timerDisplay.text = display;
    }
}
