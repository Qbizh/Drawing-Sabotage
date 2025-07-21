using FishNet;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
using FishNet.Connection;

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager instance;

    public readonly SyncList<PlayerData> players = new SyncList<PlayerData>();

    [SerializeField] LobbyMenu lobbyMenu;

    public static event Action lobbyManagerSpawned;

    private HashSet<NetworkConnection> loadedClients = new HashSet<NetworkConnection>();

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();

        if (instance == null)
        {
            instance = this;
        }
        else if (IsServerInitialized)
        {
            Destroy(gameObject);
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        InstanceFinder.SceneManager.OnClientPresenceChangeEnd += OnClientPresenceChangeEnd;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        PlayerDataHolder.instance.playerData.index = players.Count;
        AddPlayerData(PlayerDataHolder.instance.playerData);

        lobbyManagerSpawned?.Invoke();
    }

    private void OnClientPresenceChangeEnd(ClientPresenceChangeEventArgs args)
    {
        if (args.Scene.name == "Game")
        {
            loadedClients.Add(args.Connection);

            if (loadedClients.Count == players.Count)
            {
                GamePhaseManager.instance.Init();
            }
        }
    }

    [Server]
    public void LoadGame()
    {
        loadedClients.Clear();

        var sceneLoadData = new SceneLoadData("Game");
        sceneLoadData.ReplaceScenes = ReplaceOption.All;
        InstanceFinder.SceneManager.LoadGlobalScenes(sceneLoadData);
    }


    [ServerRpc(RequireOwnership =false)] 
    private void AddPlayerData(PlayerData data) // def revisit this
    {
        players.Add(data);
    }

    [Server]
    private void RemovePlayerData(PlayerData data)
    {
        players.RemoveAt(data.index);
    }
}
