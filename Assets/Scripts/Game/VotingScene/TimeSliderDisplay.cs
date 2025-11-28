using UnityEngine;
using UnityEngine.UI;
using FishNet.Object.Synchronizing;

public class TimeSliderDisplay : MonoBehaviour
{
    Slider timeSlider;
    GameObject sliderUI;

    [SerializeField] VotingPhaseHandler votingPhaseHandler;

    bool timerRunning = false;

    private void Awake()
    {
        sliderUI = transform.GetChild(0).gameObject;
        timeSlider = sliderUI.GetComponent<Slider>();

        votingPhaseHandler.votingTimer.OnChange += OnVoteTimerChanged;

       
    }

    private void OnVoteTimerChanged(SyncTimerOperation op, float last, float next, bool asServer)
    {
        if (!asServer)
        {
            if (op == SyncTimerOperation.Start)
            {
                sliderUI.SetActive(true);
                timerRunning = true;
            }
            else if (op == SyncTimerOperation.Finished)
            {
                sliderUI.SetActive(false);
                timerRunning = false;
            }
        }
    }

    private void Update()
    {
        if (timerRunning)
        {
            timeSlider.value = votingPhaseHandler.votingTimer.Elapsed / votingPhaseHandler.votingTimer.Duration;
        }
    }
}
