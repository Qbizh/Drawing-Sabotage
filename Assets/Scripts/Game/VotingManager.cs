using UnityEngine;
using FishNet.Object;
using UnityEngine.UI;
using FishNet.Connection;
using FishNet;
using System.Collections.Generic;

public class VotingManager : NetworkBehaviour
{
    [SerializeField] VotingPhaseHandler phaseHandler;

    [SerializeField] SpriteRenderer drawingDisplay;

    [SerializeField] VotingButtons votingButtons;

    private Dictionary<NetworkConnection, (VoteType, bool)> votes = new Dictionary<NetworkConnection, (VoteType, bool)>();


    private void OnEnable()
    {
        votingButtons.DisableButtons();
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
        var texture = new Texture2D(drawingDisplay.sprite.texture.width, drawingDisplay.sprite.texture.height);
        texture.LoadImage(bytes);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        texture.Apply();

        var newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

        drawingDisplay.sprite = newSprite;

        votingButtons.ResetVote();

        if (player == InstanceFinder.NetworkManager.ClientManager.Connection)
        {
            votingButtons.DisableButtons();
        }
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
