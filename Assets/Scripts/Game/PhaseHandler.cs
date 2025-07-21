using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using TMPro;

public class PhaseHandler : NetworkBehaviour
{
    [SerializeField] private TMP_Text timerDisplay;

    public float phaseTime = 15;

    public static event Action<bool> phaseStart;
    public static bool phaseActive = false;

    public readonly SyncTimer phaseTimer = new SyncTimer();     // timer that triggers phase end on finish

    private void OnEnable()
    {
        if (timerDisplay != null) 
        {
            timerDisplay.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        phaseActive = false;
        phaseTimer.OnChange -= OnTimerChanged;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        gameObject.SetActive(false);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        gameObject.SetActive(false);
    }

    [Server]
    public virtual void StartPhase()
    {
        phaseTimer.OnChange += OnTimerChanged;

        phaseActive = true;
        phaseStart?.Invoke(true);

        StartPhaseClient();
    }

    [ObserversRpc]
    private void StartPhaseClient()
    {
        phaseActive = true;
        phaseStart?.Invoke(false);
    }

    [Server]
    public void StartPhaseTimer()
    {
        timerDisplay.gameObject.SetActive(true);
        phaseTimer.StartTimer(phaseTime);

        ShowPhaseTimerClient();
    }

    [ObserversRpc]
    public void ShowPhaseTimerClient()
    {
        timerDisplay.gameObject.SetActive(true);
    }

    private void OnTimerChanged(SyncTimerOperation op, float last, float next, bool asServer)
    {
        if (asServer && op == SyncTimerOperation.Finished)
        {
            GamePhaseManager.instance.EndPhase();
        }
    }

    private void Update()
    {
        if (timerDisplay != null && !phaseTimer.Paused)
        {
            phaseTimer.Update();

            TimeSpan time = TimeSpan.FromSeconds(phaseTimer.Remaining);
            string display = time.ToString("m\\:ss");

            timerDisplay.text = display;
        }
    }
}
