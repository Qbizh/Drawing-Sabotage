using System.Collections.Generic;
using TMPro;
using UnityEngine;
using FishNet.Object.Synchronizing;
using FishNet.Object;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] RelayConnectionManager connectionManager;

    [SerializeField] GameObject lobbyButtons;

    [SerializeField] GameObject lobbyPanel;
    [SerializeField] GameObject startButton;

    [SerializeField] Transform playersPanel;
    [SerializeField] GameObject playerCardPrefab;

    [SerializeField] TMP_InputField codeInput;

    [SerializeField] TMP_Text codeDisplay;

    GameObject[] playerCards = new GameObject[6];

    public void Start()
    {
        LobbyManager.lobbyManagerSpawned += OnLobbyManagerSpawned;
    }

    public void OnLobbyManagerSpawned()
    {
        LobbyManager.instance.players.OnChange += OnPlayersChanged;
    }

    public async void OnCreatePressed()
    {
        lobbyButtons.SetActive(false);
        

        var code = await connectionManager.CreateLobby();

        if (code != null) 
        {
            codeDisplay.text = code;
            lobbyPanel.SetActive(true);
        } else
        {
            lobbyButtons.SetActive(true);
        }
    }

    public async void OnJoinPressed()
    {
        if (codeInput.text == null) return;

        lobbyButtons.SetActive(false);
        bool validCode = await connectionManager.JoinLobby(codeInput.text);

        if (validCode)
        {
            codeDisplay.text = codeInput.text;

            lobbyPanel.SetActive(true);
            startButton.SetActive(false);
        } else
        {
            lobbyButtons.SetActive(true);
        }
    }

    public void OnStartPressed()
    {
        startButton.SetActive(false);
        LobbyManager.instance.LoadGame();
    }

    private void AddPlayerCard(PlayerData data)
    {
        if (playerCards[data.index] != null)
        {
            Destroy(playerCards[data.index]);
        }

        var newPlayerCard = Instantiate(playerCardPrefab, playersPanel);

        newPlayerCard.GetComponentInChildren<TMP_Text>().text = data.name;

        playerCards[data.index] = newPlayerCard;
    }

    private void RemovePlayerCard(PlayerData data) 
    {
        Destroy(playerCards[data.index]);
        playerCards[data.index] = null;
    }

    private void OnPlayersChanged(SyncListOperation op, int index, PlayerData oldItem, PlayerData newItem, bool asServer)
    {
        if (!asServer)
        {
            if (op == SyncListOperation.Add)
            {
                AddPlayerCard(newItem);
            }
            else if (op == SyncListOperation.RemoveAt)
            {
                RemovePlayerCard(oldItem);
            }
        }
    }

}
