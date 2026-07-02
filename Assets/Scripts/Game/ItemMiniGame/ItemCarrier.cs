using System.Collections.Generic;
using System.Threading;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

[RequireComponent (typeof(RatController))]
public class ItemCarrier : NetworkBehaviour
{
    public readonly SyncVar<ItemPickup> heldItem = new SyncVar<ItemPickup>();

    List<ItemPickup> itemsInRange = new List<ItemPickup>();

    float sanePickupDistance = 5f;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (IsOwner)
        {
            InputManager.instance.onPickup += OnPickup;

            itemsInRange.Clear();
        }
    }

    private void OnDisable()
    {
        if (IsOwner)
        {
            InputManager.instance.onPickup -= OnPickup;
        }
    }

    private void Update()
    {
        if (heldItem.Value != null)
        {
            heldItem.Value.transform.position = transform.position + transform.up * 0.5f;
        }
    }

    public void AddItemInRange(ItemPickup item)
    {
        if (!IsOwner) return;

        if (!itemsInRange.Contains(item))
        {
            itemsInRange.Add(item);
        }
    }

    public void RemoveItemFromRange(ItemPickup item)
    {
        if (!IsOwner) return;

        if (itemsInRange.Contains(item)) 
        {
            itemsInRange.Remove(item);
        }
    }

    private void OnPickup()
    {
        if (itemsInRange.Count == 0) return;

        ItemPickup closest = null;

        float smallestDist = float.MaxValue;

        var itemsToRemove = new List<ItemPickup>();

        foreach (var item in itemsInRange) 
        {
            if (item.carrier.Value != null)
            {
                itemsToRemove.Add(item);
                continue;
            }

            float dist = (transform.position - item.transform.position).magnitude;

            if (dist < smallestDist)
            {
                smallestDist = dist;
                closest = item;
            }
        }

        foreach (var item in itemsToRemove) 
        {
            itemsInRange.Remove(item);
        }

        if (closest != null)
        {
            Debug.Log(closest);
            TryItemPickup(closest);
        }
    }

    [ServerRpc]
    private void TryItemPickup(ItemPickup item)
    {
        float dist = (item.transform.position - transform.position).magnitude;

        if (dist <= sanePickupDistance)
        {
            if (item.carrier.Value == null && !item.Owner.IsValid)
            {
                if (heldItem.Value != null) 
                {
                    heldItem.Value.SetCarrier(null);
                    heldItem.Value.RemoveOwnership();
                    heldItem.Value = null;
                }

                item.SetCarrier(this);
                item.GiveOwnership(Owner);

                heldItem.Value = item;

                OnPickupSuccess(Owner);
            }
            else
            {
                Debug.Log(Owner);
            }
        }
        else
        {
            Debug.LogWarning("Suspicious item collision detected for client " + OwnerId);
        }
    }

    [TargetRpc]
    private void OnPickupSuccess(NetworkConnection conn)
    {
        itemsInRange.Remove(heldItem.Value);
    }
}
