using UnityEngine;
using FishNet.Object;
using static UnityEditor.Progress;
using FishNet.Connection;
using FishNet.Object.Synchronizing;

public class ItemPickup : NetworkBehaviour
{
    public CardData itemData;

    [SerializeField] CardDatabase itemDatabase;

    [SerializeField] SpriteRenderer itemRenderer;



    public readonly SyncVar<ItemCarrier> carrier = new SyncVar<ItemCarrier>();

    [Server]
    public void SetItem(CardData item)
    {
        SetUp(item);

        SetItemClient(itemDatabase.GetCardId(item));
    }

    [ObserversRpc]
    private void SetItemClient(int itemId)
    {
        SetUp(itemDatabase.GetCard(itemId));
    }

    private void SetUp(CardData item)
    {
        itemData = item;

        itemRenderer.sprite = itemData.GenerateSprite(itemRenderer.sprite.texture.width, itemRenderer.sprite.texture.height);
    }

    private void Update()
    {
        if (carrier.Value != null)
        {
            transform.position = carrier.Value.transform.position + carrier.Value.transform.up * 0.5f;
        }
    }

    [Server]
    public void SetCarrier(ItemCarrier newCarrier)
    {
        carrier.Value = newCarrier;
    }

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (carrier.Value != null || Owner.IsValid) return;

        ItemCarrier rat;

        if (other.TryGetComponent<ItemCarrier>(out rat))
        {
            rat.AddItemInRange(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (carrier.Value != null || Owner.IsValid) return;

        ItemCarrier rat;

        if (other.TryGetComponent<ItemCarrier>(out rat))
        {
            rat.RemoveItemFromRange(this);
        }
    }
}
