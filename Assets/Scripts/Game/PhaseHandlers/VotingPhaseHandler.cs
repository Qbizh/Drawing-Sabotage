using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class VotingPhaseHandler : PhaseHandler
{
    [SerializeField] VotingManager votingManager;

    Dictionary<NetworkConnection, Dictionary<NetworkConnection, (VoteType, bool)>> allVotes = new Dictionary<NetworkConnection, Dictionary<NetworkConnection, (VoteType, bool)>>();

    public NetworkConnection currentPlayer;

    Queue<NetworkConnection> playerQueue;

    public readonly SyncTimer votingTimer = new SyncTimer();
    [SerializeField] float votingTime = 10f;

    [Server]
    public override void SetUpPhase()
    {
        allVotes.Clear();

        var players = new List<NetworkConnection>(LobbyManager.instance.players.Keys);
        ListOperations.Shuffle(players);

        playerQueue = new Queue<NetworkConnection>(players);

        base.SetUpPhase();
    }

    [Server]
    public override void StartPhase()
    {
        SetUpNextVote();

        base.StartPhase();
    }

    [Server]
    private void SetUpNextVote()
    {
        if (playerQueue.Count != 0)
        {
            currentPlayer = playerQueue.Dequeue();

            votingManager.SetUpVote(currentPlayer, GamePhaseManager.instance.gameDataHolder.playerDrawings[currentPlayer]);

            votingTimer.StartTimer(votingTime);
            votingTimer.OnChange += OnVoteTimerChanged;
        } else
        {
            Debug.Log("EVERYTHING WAS VOTED ON");
        }
    }

    private void OnVoteTimerChanged(SyncTimerOperation op, float last, float next, bool asServer)
    {
        if (op == SyncTimerOperation.Finished && asServer)
        {
            votingTimer.OnChange -= OnVoteTimerChanged;

            votingManager.RequestVotes();
        }
    }

    [Server]
    public void SetVotes(Dictionary<NetworkConnection, (VoteType, bool)> roundVotes)
    {
        if (allVotes.TryAdd(currentPlayer, roundVotes))
        {
            SetUpNextVote();
        } else
        {
            Debug.LogError("Error adding votes");
        }
    }

    public override void Update()
    {
        base.Update();

        if (!votingTimer.Paused)
        {
            votingTimer.Update();

            /*if (timerDisplay != null)
            {
                TimeSpan time = TimeSpan.FromSeconds(phaseTimer.Remaining);
                string display = time.ToString("m\\:ss");

                timerDisplay.text = display;
            }*/
        }
    }
}
