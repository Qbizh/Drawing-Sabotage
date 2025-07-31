using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using System.Collections.Generic;
using FishNet;

public class DrawingPhaseHandler : PhaseHandler
{
    private Dictionary<NetworkConnection, byte[]> playerDrawings = new Dictionary<NetworkConnection, byte[]>();

    [Server]
    public override void StartPhase()
    {
        playerDrawings.Clear();

        base.StartPhase();

        phaseTimerFinished += OnPhaseTimerFinished;

        StartPhaseTimer();
    }

    private void OnPhaseTimerFinished(bool asServer)
    {
        phaseTimerFinished -= OnPhaseTimerFinished;

        if (!asServer)
        {
            var myDrawing = DrawingBoard.GetMainBoard().texture.EncodeToPNG();
            AddDrawing(InstanceFinder.ClientManager.Connection, myDrawing);
        }
    }

    [ServerRpc]
    private void AddDrawing(NetworkConnection conn, byte[] drawing)
    {
        if (playerDrawings.TryAdd(conn, drawing))
        {
            if (playerDrawings.Count == LobbyManager.instance.players.Count)
            {
                GamePhaseManager.instance.gameDataHolder.SetPlayerDrawings(playerDrawings);
            }
        }
    }

}
