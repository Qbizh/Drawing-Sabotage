using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet;


public class PromptGenerationPhaseHandler : PhaseHandler
{
    [SerializeField] GameObject hostUI;
    GameDataHolder.PromptData currentPromptData;

    [Server]
    public override void StartPhase()
    {
        base.StartPhase();

        HideHostUI(false);
    }

    public void LockInPrompt()
    {
        if (IsHostInitialized)
        {
            SetPrompt();
        }
    }

    [Server]
    public GameDataHolder.PromptData GeneratePrompt()
    {
        currentPromptData = GamePhaseManager.instance.gameDataHolder.GeneratePrompt();

        return currentPromptData;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPrompt()
    {
        if (!string.IsNullOrEmpty(currentPromptData.prompt))
        {
            GamePhaseManager.instance.gameDataHolder.SetPrompt(currentPromptData);

            HideHostUI(true);
            StartPhaseTimer();
        }
    }

    [ObserversRpc]
    private void HideHostUI(bool hideForHost)
    {
        if (!IsHostInitialized)
        {
            hostUI.SetActive(false);
        } else
        {
            hostUI.SetActive(hideForHost ? false : true);
        }
    }

    public override void SkipPhase()
    {
        if (!string.IsNullOrEmpty(currentPromptData.prompt))
        {
            GamePhaseManager.instance.gameDataHolder.SetPrompt(currentPromptData);

            HideHostUI(true);
        }

        base.SkipPhase();
    }
}
