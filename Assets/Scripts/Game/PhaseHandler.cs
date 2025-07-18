using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using TMPro;

public class PhaseHandler : NetworkBehaviour
{
    [SerializeField] private TMP_Text timerDisplay;

    public readonly SyncTimer phaseTimer = new SyncTimer();

    public void Init()
    {
        phaseTimer.OnChange += OnTimerChanged;
    }

    private void OnDisable()
    {
        phaseTimer.OnChange -= OnTimerChanged;
    }

    public void StartPhase()
    {
        

    }

    private void OnTimerChanged(SyncTimerOperation op, float last, float next, bool asServer)
    {
        if (asServer && op == SyncTimerOperation.Finished)
        {
            
        }
    }

    private void Update()
    {
        if (!phaseTimer.Paused)
        {
            phaseTimer.Update();

            TimeSpan time = TimeSpan.FromSeconds(phaseTimer.Remaining);
            string display = time.ToString("m\\:ss");

            timerDisplay.text = display;
        }
    }
}
