using UnityEngine;
using Unity.Services.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using FishNet;
using FishNet.Transporting.UTP;
using System.Threading.Tasks;

public class RelayConnectionManager : MonoBehaviour, IConnectionManager
{
    string connectionType = "udp";

    int maxPlayers = 6;

    [SerializeField] GameObject lobbyManagerPrefab;
    LobbyManager lobbyManager;

    public string connectedJoinCode { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Init();
    }

    public async void Init()
    {
        await UnityServices.InitializeAsync();
        
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    public async Task<string> CreateLobby()
    {
        var allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
        InstanceFinder.NetworkManager.GetComponent<UnityTransport>().SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);

        var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        connectedJoinCode = joinCode;

        if (InstanceFinder.ServerManager.StartConnection() && InstanceFinder.ClientManager.StartConnection())
        {

            lobbyManager = Instantiate(lobbyManagerPrefab).GetComponent<LobbyManager>();
            InstanceFinder.ServerManager.Spawn(lobbyManager.gameObject);

            return joinCode;
        } else
        {
            connectedJoinCode = null;
            return null;
        }
    }

    public async Task<bool> JoinLobby(string joinCode)
    {
        JoinAllocation allocation = null;

        try
        {
            allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            InstanceFinder.NetworkManager.GetComponent<UnityTransport>().SetClientRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);

            connectedJoinCode = joinCode;

            InstanceFinder.ClientManager.StartConnection();

            return true;
        } 
        catch (RelayServiceException ex) 
        {
            Debug.LogException(ex);

            connectedJoinCode = null;

            return false;
        }
    }
}
