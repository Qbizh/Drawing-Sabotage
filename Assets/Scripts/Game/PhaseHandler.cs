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
    public static event Action<bool> phaseSetUp;

    public static event Action<bool> phaseTimerStart;
    public static event Action<bool> phaseTimerFinished;

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
    public virtual void SetUpPhase()
    {
        phaseSetUp?.Invoke(true);
        SetUpPhaseClient();
    }

    [ObserversRpc]
    private void SetUpPhaseClient()
    {
        phaseSetUp?.Invoke(false);
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
        timerDisplay?.gameObject.SetActive(true);
        phaseTimer.StartTimer(phaseTime);

        phaseTimerStart?.Invoke(true);

        ShowPhaseTimerClient();
    }

    [ObserversRpc]
    public void ShowPhaseTimerClient()
    {
        timerDisplay?.gameObject.SetActive(true);
        phaseTimerStart?.Invoke(false);
    }

    private void OnTimerChanged(SyncTimerOperation op, float last, float next, bool asServer)
    {
        if (op == SyncTimerOperation.Finished)
        {
            phaseTimerFinished?.Invoke(asServer);

            timerDisplay?.gameObject.SetActive(false);

            if (asServer)
            {
                GamePhaseManager.instance.EndPhase();
            }
        }
    }

    private void Update()
    {
        if (!phaseTimer.Paused)
        {
            phaseTimer.Update();

            if (timerDisplay != null)
            {
                TimeSpan time = TimeSpan.FromSeconds(phaseTimer.Remaining);
                string display = time.ToString("m\\:ss");

                timerDisplay.text = display;
            }
        }
    }
}
