using System;
using System.Collections.Generic;
using System.Timers;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using static GameManager;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    public static event Action<GameState, bool> gameStateStart;

    GameObject[] gameScenes = new GameObject[3];

    float[] periodTimes = new float[3];

    [SerializeField] CardDatabase cardDatabase;

    [SerializeField] TMP_Text timerDisplay;

    [SerializeField] Animator scenesAnimator;

    public readonly SyncTimer periodTimer = new SyncTimer();

    Texture2D[] playerDrawings;

    HashSet<PlayerData> loadedPlayers;
    
    public enum GameState 
    { 
        Prompt,
        Game,
        Voting
    }

    int currentRound = 0;

    public readonly SyncVar<GameState> gameState = new SyncVar<GameState>();
    public readonly SyncVar<GameState> lastGameState = new SyncVar<GameState>();

    private GameObject currentScene;

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

    public override void OnStartServer()
    {
        base.OnStartServer();

        loadedPlayers = new HashSet<PlayerData>(LobbyManager.instance.players.Count);
    }

    public void Init()
    {
        periodTimer.OnChange += OnTimerChanged;

        InitializeRound();
    }

    private void InitializeRound()
    {
        currentRound++;

        if (currentRound == 1)
        {
            gameState.Value = GameState.Prompt;
            currentScene = gameScenes[(int)gameState.Value];
            currentScene.SetActive(true);

            TransitionToFirstRound();
        } else
        {
            TransitionState(GameState.Prompt);
        }
    }

    [ObserversRpc]
    private void TransitionToFirstRound()
    {
        currentScene = gameScenes[(int)gameState.Value];
        currentScene.SetActive(true);

        scenesAnimator.SetTrigger("FirstLoad");
    }

    private void OnTimerChanged(SyncTimerOperation op, float last, float next, bool asServer)
    {
        if (asServer && op == SyncTimerOperation.Finished) 
        {
            Debug.Log("PERIOD OVER");

            if (gameState.Value == GameState.Voting)
            {
                // collect results here 

                InitializeRound();
            }
        }
    }

    private void Update()
    {
        if (!periodTimer.Paused)
        {
            periodTimer.Update();

            TimeSpan time = TimeSpan.FromSeconds(periodTimer.Remaining);
            string display = time.ToString("m\\:ss");

            timerDisplay.text = display;
        }
    }

    [Server]
    private void TransitionState(GameState newState)
    {
        lastGameState.Value = gameState.Value;
        gameState.Value = newState;

        StartClientLoad();
    }


    [Server]
    private bool AllClientsLoaded(PlayerData player)
    {
        loadedPlayers.Add(player);

        if (loadedPlayers.Count == LobbyManager.instance.players.Count)
        {
            loadedPlayers.Clear();

            return true;
        }

        return false;
    }

    [ObserversRpc]
    private void StartClientLoad()
    {
        scenesAnimator.SetTrigger("Load");
    }



    public void OnLoadIn()
    {
        scenesAnimator.speed = 0;

        ClientLoadedIn(PlayerDataHolder.instance.playerData);
    }

    [ServerRpc]
    private void ClientLoadedIn(PlayerData player)
    {
        if (AllClientsLoaded(player)) 
        {
            if (currentScene != null) 
            {
                currentScene.SetActive(false);
            }

            currentScene = gameScenes[(int)gameState.Value];

            currentScene.SetActive(true);

            ContinueClientLoad();
        }
    }


    [ObserversRpc]
    private void ContinueClientLoad()
    {
        if (currentScene != null)
        {
            currentScene.SetActive(false);
        }

        currentScene = gameScenes[(int)gameState.Value];

        currentScene.SetActive(true);

        scenesAnimator.speed = 1;
    }

    public void OnLoadOut()
    {
        ClientFinishedLoad(PlayerDataHolder.instance.playerData);
    }

    [ServerRpc]
    private void ClientFinishedLoad(PlayerData player)
    {
        if (AllClientsLoaded(player))
        {
            gameStateStart?.Invoke(gameState.Value, true); // currentState, isServer
            StartStateClient();
        }
    }

    [ObserversRpc]
    private void StartStateClient()
    {
        gameStateStart?.Invoke(gameState.Value, false);
    }
   
}
