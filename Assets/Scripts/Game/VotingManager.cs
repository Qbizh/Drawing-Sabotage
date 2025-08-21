using UnityEngine;
using FishNet.Object;
using UnityEngine.UI;
using FishNet.Connection;
using FishNet;
using System.Collections.Generic;

public class VotingManager : NetworkBehaviour
{
    [SerializeField] VotingPhaseHandler phaseHandler;

    [SerializeField] Image drawingDisplay;

    [SerializeField] VotingButtons votingButtons;

    private Dictionary<NetworkConnection, (VoteType, bool)> votes = new Dictionary<NetworkConnection, (VoteType, bool)>();


    private void OnEnable()
    {
        votingButtons.DisableButtons();
    }

    [Server]
    public void SetUpVote(byte[] drawing)
    {
        votes.Clear();

        SetUpClient(drawing);
    }

    [ObserversRpc]
    private void SetUpClient(byte[] bytes)
    {
        var texture = new Texture2D(drawingDisplay.mainTexture.width, drawingDisplay.mainTexture.height);
        texture.LoadImage(bytes);
        texture.Apply();

        var newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

        drawingDisplay.sprite = newSprite;

        votingButtons.ResetVote();
    }

    [Server]
    public void RequestVotes()
    {
        GetVoteClient();
    }

    [ServerRpc]
    private void WaitForVotes(NetworkConnection conn, VoteType vote, bool invested)
    {
        if (votes.TryAdd(conn, (vote, invested))) 
        {
            if (votes.Count == LobbyManager.instance.players.Count)
            {
                phaseHandler.SetVotes(votes);
            }
        }
    }

    [ObserversRpc]
    private void GetVoteClient()
    {
        votingButtons.DisableButtons();

        WaitForVotes(InstanceFinder.ClientManager.Connection,  votingButtons.currentVote, votingButtons.invested);
    }
}
