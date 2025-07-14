using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet;

public class PipesManager : NetworkBehaviour
{
    public static PipesManager instance;

    [SerializeField] Transform itemSpawn;

    [SerializeField] GameObject[] pipes;

    [SerializeField] CardDatabase cardDatabase;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this);
        }
    }

    private void OnEnable()
    {
        GameManager.gameStateStart += OnGameStart;
    }

    private void OnDisable()
    {
        GameManager.gameStateStart -= OnGameStart;
    }

    private void OnGameStart(GameManager.GameState state, bool asServer)
    {
        if (!asServer)
        {
            foreach (var client in InstanceFinder.ClientManager.Clients)
            {
                if (client.Value != InstanceFinder.ClientManager.Connection)
                {
                    AddPlayerPipe(client.Value);
                }
            }
        }
    }

    public void AddPlayerPipe(NetworkConnection client)
    {
        for (int i = 0; i < pipes.Length; i++)
        {
            if (!pipes[i].activeInHierarchy)
            {
                pipes[i].gameObject.SetActive(true);
                pipes[i].GetComponent<Pipe>().SetPlayer(client);

                break;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendItemToClient(NetworkConnection targetClient, int cardId, float cardScore, byte[] textureBytes)
    {
        RecieveItem(targetClient, cardId, cardScore, textureBytes);
    }

    [TargetRpc]
    private void RecieveItem(NetworkConnection targetClient, int cardId, float cardScore, byte[] textureBytes)
    {
        CardData data = cardDatabase.GetCard(cardId);

        var item = Instantiate(data.prefab);
        item.GetComponent<ItemBehaviour>().Initialize(textureBytes, cardScore, itemSpawn.position);

        item.GetComponent<ItemBehaviour>().Deploy();
    }
}
