using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using TMPro;
using UnityEngine;

public class GamePhaseManager : NetworkBehaviour
{
    public static GamePhaseManager instance;
    
    public GameDataHolder gameDataHolder;

    [SerializeField] PhaseHandler[] phaseHandlers = new PhaseHandler[4];
    [SerializeField] GameObject loadingScene;

    [SerializeField] CardDatabase cardDatabase;

    [SerializeField] TMP_Text timerDisplay;

    [SerializeField] Animator scenesAnimator;

    Texture2D[] playerDrawings;

    HashSet<PlayerData> loadedPlayers;
    
    public enum GamePhase 
    { 
        PromptInput,
        PromptGeneration,
        Drawing,
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

        gameDataHolder = GetComponent<GameDataHolder>();

        /*for (int i = 0; i < phaseHandlers.Length; i++)
        {
            phaseHandlers[i].gameObject.SetActive(false);
        }*/

        loadingScene.SetActive(true);
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
            gamePhase.Value = GamePhase.PromptInput;
            currentPhaseHandler = phaseHandlers[(int)gamePhase.Value];
            currentPhaseHandler.gameObject.SetActive(true);

            TransitionToFirstRound();
        } else
        {
            if (gameDataHolder.AssessFormats())
            {
                TransitionState(GamePhase.PromptGeneration);
            } else
            {
                TransitionState(GamePhase.PromptInput);
            }
        }
    }

    [ObserversRpc]
    private void TransitionToFirstRound()
    {
        currentPhaseHandler = phaseHandlers[(int)gamePhase.Value];
        currentPhaseHandler.gameObject.SetActive(true);


        scenesAnimator.SetTrigger("FirstLoad");
        scenesAnimator.speed = 0;

        ClientChangedPhase(PlayerDataHolder.instance.playerData, true);
    }

    [Server]
    public void EndPhase()
    {
        if (gamePhase.Value == GamePhase.Voting)
        {
            InitializeRound();
        }
        else
        {
            TransitionState((gamePhase.Value + 1));
        }
    }

    [Server]
    private void TransitionState(GamePhase newState)
    {
        lastGamePhase.Value = gamePhase.Value;
        gamePhase.Value = newState;

        Debug.Log("Transitioning to " + gamePhase.Value);

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

            ChangeCurrentPhase();
        }
    }


    [ObserversRpc]
    private void ChangeCurrentPhase()
    {
        if (currentPhaseHandler != null)
        {
            currentPhaseHandler.gameObject.SetActive(false);
        }

        currentPhaseHandler = phaseHandlers[(int)gamePhase.Value];

        currentPhaseHandler.gameObject.SetActive(true);

        ClientChangedPhase(PlayerDataHolder.instance.playerData, false);
    }

    [ServerRpc(RequireOwnership =false)]
    private void ClientChangedPhase(PlayerData player, bool firstRound)
    {
        if (AllClientsLoaded(player))
        {
            currentPhaseHandler.SetUpPhase();
            ContinueClientLoad();
        }
    }

    [ObserversRpc]
    private void ContinueClientLoad()
    {
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
            currentPhaseHandler.StartPhase();
        }
    }

    public void OnSkipPhase()
    {
        if (IsHostInitialized)
        {
            currentPhaseHandler.SkipPhase();
        }
    }
}
