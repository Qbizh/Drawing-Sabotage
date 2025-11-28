using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class VotingPhaseHandler : PhaseHandler
{
    [SerializeField] VotingManager votingManager;

    Dictionary<NetworkConnection, Dictionary<NetworkConnection, (VoteType, bool)>> allVotes = new Dictionary<NetworkConnection, Dictionary<NetworkConnection, (VoteType, bool)>>();
    Dictionary<NetworkConnection, int> roundScores = new Dictionary<NetworkConnection, int>();

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
            ScoreVotes();
        }
    }

    [Server]
    private void ScoreVotes()
    {
        Debug.Log(allVotes.Count);

        foreach (var player in allVotes.Keys)
        {
            var votes = allVotes[player];

            int score = 0;

            List<NetworkConnection> investedVoters = new List<NetworkConnection>();

            foreach (var voter in votes.Keys)
            {
                var vote = votes[voter];

                switch (vote.Item1) 
                {
                    case VoteType.Up:
                        score += VotingConstants.upVoteScore;
                        break;
                    case VoteType.Down:
                        score += VotingConstants.downVoteScore;
                        break;
                    case VoteType.Meh:
                        score += VotingConstants.mehVoteScore;
                        break;
                }

                if (vote.Item2)
                {
                    investedVoters.Add(voter);
                }
            }

            foreach (var voter in investedVoters)
            {
                int investmentReturn = Mathf.RoundToInt(score * VotingConstants.investmentReturnPercentage);

                AddScore(voter, investmentReturn);
            }

            AddScore(player, score);
        }

        votingManager.DisplayResults(roundScores);

        GamePhaseManager.instance.gameDataHolder.AddRoundScores(roundScores);

        StartPhaseTimer();
    } 

    private void AddScore(NetworkConnection player, int add)
    {
        int currentScore;

        if (roundScores.TryGetValue(player, out currentScore))
        {
            roundScores[player] = currentScore + add;
        }
        else
        {
            roundScores.Add(player, add);
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
        }
    }
}
