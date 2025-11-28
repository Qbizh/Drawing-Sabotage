using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet;
using System.Collections.Generic;
using TMPro;

public class VotingManager : NetworkBehaviour
{
    [SerializeField] VotingPhaseHandler phaseHandler;

    [SerializeField] SpriteRenderer drawingDisplay;

    [SerializeField] VotingButtons votingButtons;
    [SerializeField] TMP_Text promptText;

    [SerializeField] List<GameObject> votingObjects = new List<GameObject>();


    [SerializeField] GameObject playerResultsPrefab;
    [SerializeField] GameObject resultsGrid;

    private Dictionary<NetworkConnection, (VoteType, bool)> votes = new Dictionary<NetworkConnection, (VoteType, bool)>();


    private void OnEnable()
    {
        votingButtons.DisableButtons();

        PhaseHandler.phaseStart += OnPhaseStart;
    }

    private void OnPhaseStart(bool asServer)
    {
        if (GamePhaseManager.instance.gamePhase.Value == GamePhaseManager.GamePhase.Voting)
        {
            if (asServer)
            {
                PhaseHandler.phaseTimerFinished += OnPhaseEnd;
            } else
            {
                promptText.text = GamePhaseManager.instance.gameDataHolder.currentPrompt.Value;
            }
        }
    }

    [Server]
    private void OnPhaseEnd()
    {
        PhaseHandler.phaseTimerFinished -= OnPhaseEnd;

        SetVotingEnabled(true);
        HideResults();
    }

    [Server]
    public void SetUpVote(NetworkConnection player, byte[] drawing)
    {
        votes.Clear();

        SetUpClient(player, drawing);
    }

    [ObserversRpc]
    private void SetUpClient(NetworkConnection player, byte[] bytes)
    {
        drawingDisplay.sprite = CreateSpriteFromDrawing(bytes);

        votingButtons.ResetVote();

        if (player == InstanceFinder.NetworkManager.ClientManager.Connection)
        {
            votingButtons.DisableButtons();
        }
    }

    private Sprite CreateSpriteFromDrawing(byte[] bytes)
    {
        var texture = new Texture2D(drawingDisplay.sprite.texture.width, drawingDisplay.sprite.texture.height);
        texture.LoadImage(bytes);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
    }

    [Server]
    public void RequestVotes()
    {
        GetVoteClient();
    }

    [ServerRpc(RequireOwnership = false)]
    private void WaitForVotes(NetworkConnection conn, VoteType vote, bool invested)
    {
        if (votes.TryAdd(conn, (vote, invested))) 
        {
            if (votes.Count == LobbyManager.instance.players.Count)
            {
                phaseHandler.SetVotes(new Dictionary<NetworkConnection, (VoteType, bool)>(votes));
            }
        }
    }

    [ObserversRpc]
    private void GetVoteClient()
    {
        votingButtons.DisableButtons();

        Debug.Log(LobbyManager.instance.players[InstanceFinder.ClientManager.Connection].name + ": " + votingButtons.currentVote);

        WaitForVotes(InstanceFinder.ClientManager.Connection,  votingButtons.currentVote, votingButtons.invested);
    }



    [ObserversRpc]
    public void DisplayResults(Dictionary<NetworkConnection, int> scores)
    {
        SetVotingEnabled(false);
        resultsGrid.SetActive(true);

        foreach (var player in scores.Keys)
        {
            var score = scores[player];
            var drawingSprite = CreateSpriteFromDrawing(GamePhaseManager.instance.gameDataHolder.playerDrawings[player]);
            var name = LobbyManager.instance.players[player].name;

            var playerResults = Instantiate(playerResultsPrefab);
            playerResults.GetComponent<PlayerResultsDisplay>().SetUp(name, score, drawingSprite);

            playerResults.transform.SetParent(resultsGrid.transform, false);
        }
    }

    [ObserversRpc]
    private void HideResults()
    {
        resultsGrid.SetActive(false);

        foreach (Transform resultsObj in resultsGrid.transform)
        {
            Destroy(resultsObj.gameObject);
        }
    }

    private void SetVotingEnabled(bool enabled)
    {
        votingButtons.HideButtons(enabled);

        foreach (var obj in votingObjects)
        {
            obj.SetActive(enabled);
        }
    }
}
