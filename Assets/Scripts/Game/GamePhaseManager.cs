using System;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using TMPro;
using UnityEngine;

public class GamePhaseManager : NetworkBehaviour
{
    public static GamePhaseManager instance;

    public static event Action<GamePhase, bool> GamePhaseStart;
    public static bool phaseActive = false;

    [SerializeField] PhaseHandler[] phaseHandlers = new PhaseHandler[3];
    [SerializeField] GameObject loadingScene;

    [SerializeField] float[] phaseTimes = new float[3];

    [SerializeField] CardDatabase cardDatabase;

    [SerializeField] TMP_Text timerDisplay;

    [SerializeField] Animator scenesAnimator;

    Texture2D[] playerDrawings;

    HashSet<PlayerData> loadedPlayers;
    
    public enum GamePhase 
    { 
        Prompt,
        Game,
        Voting
    }

    int roundAmount = 3;
    int currentRound = 0;

    public readonly SyncVar<GamePhase> gamePhase = new SyncVar<GamePhase>();
    public readonly SyncVar<GamePhase> lastGamePhase = new SyncVar<GamePhase>();

    private PhaseHandler currentPhaseHandler;

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

        for (int i = 0; i < phaseHandlers.Length; i++)
        {
            phaseHandlers[i].gameObject.SetActive(false);
        }

        loadingScene.SetActive(true);
        
        GamePhaseStart += OnGamePhaseStart;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        loadedPlayers = new HashSet<PlayerData>(LobbyManager.instance.players.Count);
    }

    public void Init()
    {
        InitializeRound();
    }

    private void InitializeRound()
    {
        currentRound++;

        if (currentRound == 1)
        {
            gamePhase.Value = GamePhase.Prompt;
            currentPhaseHandler = phaseHandlers[(int)gamePhase.Value];
            currentPhaseHandler.gameObject.SetActive(true);

            TransitionToFirstRound();
        } else
        {
            TransitionState(GamePhase.Prompt);
        }
    }

    [ObserversRpc]
    private void TransitionToFirstRound()
    {
        currentPhaseHandler = phaseHandlers[(int)gamePhase.Value];
        currentPhaseHandler.gameObject.SetActive(true);


        scenesAnimator.SetTrigger("FirstLoad");
    }

    private void OnGamePhaseStart(GamePhase state, bool asServer)
    {
        phaseActive = true;

        if (asServer)
        {
            //phaseTimer.StartTimer(phaseTimes[(int)state]);
        }
    }

    private void OnTimerChanged(SyncTimerOperation op, float last, float next, bool asServer)
    {
        if (asServer && op == SyncTimerOperation.Finished) 
        {
            phaseActive = false;

            if (gamePhase.Value == GamePhase.Voting)
            {
                // collect results here 

                InitializeRound();
            } else
            {
                TransitionState((gamePhase.Value + 1));
            }
        }
    }

    [Server]
    private void TransitionState(GamePhase newState)
    {
        lastGamePhase.Value = gamePhase.Value;
        gamePhase.Value = newState;

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

    [ServerRpc(RequireOwnership = false)]
    private void ClientLoadedIn(PlayerData player)
    {
        if (AllClientsLoaded(player)) 
        {
            if (currentPhaseHandler != null) 
            {
                currentPhaseHandler.gameObject.SetActive(false);
            }

            currentPhaseHandler = phaseHandlers[(int)gamePhase.Value];

            currentPhaseHandler.gameObject.SetActive(true);

            ContinueClientLoad();
        }
    }


    [ObserversRpc]
    private void ContinueClientLoad()
    {
        if (currentPhaseHandler != null)
        {
            currentPhaseHandler.gameObject.SetActive(false);
        }

        currentPhaseHandler = phaseHandlers[(int)gamePhase.Value];

        currentPhaseHandler.gameObject.SetActive(true);

        scenesAnimator.speed = 1;
    }

    public void OnLoadOut()
    {
        ClientFinishedLoad(PlayerDataHolder.instance.playerData);
    }

    [ServerRpc(RequireOwnership =false)]
    private void ClientFinishedLoad(PlayerData player)
    {
        if (AllClientsLoaded(player))
        {
            GamePhaseStart?.Invoke(gamePhase.Value, true); // currentState, isServer
            StartStateClient();
        }
    }

    [ObserversRpc]
    private void StartStateClient()
    {
        GamePhaseStart?.Invoke(gamePhase.Value, false);
    }
   
}
