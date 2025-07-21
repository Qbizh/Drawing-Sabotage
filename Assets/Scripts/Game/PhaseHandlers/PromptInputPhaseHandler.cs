using UnityEngine;
using System.Collections.Generic;
using FishNet.Object;
using System.Linq;

public class PromptInputPhaseHandler : PhaseHandler
{
    public HashSet<string> tags = new HashSet<string>();

    [Server]
    public override void StartPhase()
    {
        tags = GamePhaseManager.instance.gameDataHolder.GetUnfulfilledFormatTags();

        UpdateTagsClient(tags.ToList());

        base.StartPhase();  
    }

    [ObserversRpc]
    public void UpdateTagsClient(List<string> newTags)
    {
        tags = new HashSet<string>(newTags);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddPromptInput(string tag, string input)
    {
        if (tags.Contains(tag))
        {

            bool aFormatIsFulfilled = GamePhaseManager.instance.gameDataHolder.AddPromptInput(tag, input);
            
            bool phaseTimerStarted = phaseTimer.Elapsed != phaseTimer.Duration;
            
            Debug.Log(phaseTimerStarted);
            if (aFormatIsFulfilled && !phaseTimerStarted)        // returns true if the added input makes a format fulfilled meaning we can start the countdown to next phase
            {
                StartPhaseTimer();
            }
        }
    }
}