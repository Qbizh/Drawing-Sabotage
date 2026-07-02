using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;

public class RatManager : NetworkBehaviour
{
    [SerializeField] NetworkObject ratPrefab;

    List<NetworkObject> playerRats = new List<NetworkObject>();

    [SerializeField] List<RatSpawn> spawns = new List<RatSpawn>();

    private void OnEnable()
    {
        PhaseHandler.phaseSetUp += SpawnRats;
        PhaseHandler.phaseTimerFinished += DespawnRats;
    }

    private void OnDisable()
    {
        PhaseHandler.phaseSetUp -= SpawnRats;
        PhaseHandler.phaseTimerFinished -= DespawnRats;
    }

    public void SpawnRats(bool isServer)
    {
        if (isServer) 
        {
            int index = 0;

            foreach (var player in LobbyManager.instance.players.Keys)
            {
                var spawn = spawns[index];

                spawn.gameObject.SetActive(true);
                SetSpawnActive(index, true);

                spawn.GiveOwnership(player);

                var playerRat = Instantiate(ratPrefab);

                playerRat.transform.position = spawn.GetSpawnPosition();

                playerRats.Add(playerRat);

                Spawn(playerRat, player);

                index++;
            }
        }
    }

    [Server]
    private void DespawnRats()
    {
        foreach (var rat in playerRats)
        {
            rat.Despawn();
        }

        for (int i = 0; i < spawns.Count; i++)
        {
            spawns[i].RemoveOwnership();

            spawns[i].gameObject.SetActive(false);
            SetSpawnActive(i, false);
        }
    }

    [ObserversRpc]
    private void SetSpawnActive(int index, bool active)
    {
        spawns[index].gameObject.SetActive(active);
    }
}
