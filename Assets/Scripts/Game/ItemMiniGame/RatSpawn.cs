
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class RatSpawn : NetworkBehaviour
{
    [SerializeField] Transform entryPoint;

    private void OnEnable()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsOwner)
        {
            ItemCarrier ratCarrier;

            if (other.TryGetComponent<ItemCarrier>(out ratCarrier))
            {
                if (ratCarrier.Owner == Owner)
                {
                    
                }
            }
        }
    }

    public Vector2 GetSpawnPosition()
    {
        return entryPoint.position;
    }
}
