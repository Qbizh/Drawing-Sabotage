using FishNet.Object;
using UnityEngine;

public class ItemPickupSpawner : NetworkBehaviour
{
    [SerializeField] NetworkObject itemPickupPrefab;

    NetworkObject spawnedItemPickup;

    private void OnEnable()
    {
        PhaseHandler.phaseSetUp += OnPhaseSetup;
    }

    private void OnDisable()
    {
        PhaseHandler.phaseSetUp -= OnPhaseSetup;
    }


    private void OnServerInitialized()
    {
        
    }

    private void OnPhaseSetup(bool asServer)
    {
        if (asServer) 
        {
            spawnedItemPickup = Instantiate(itemPickupPrefab);
            spawnedItemPickup.transform.position = transform.position;

            Debug.Log("Spawned an item");
            Spawn(spawnedItemPickup);
        }
    }
}
