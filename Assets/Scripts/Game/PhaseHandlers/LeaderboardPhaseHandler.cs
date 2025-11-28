using UnityEngine;
using FishNet.Object;

public class LeaderboardPhaseHandler : PhaseHandler
{

    [Server]
    public override void StartPhase()
    {

        StartPhaseTimer();

        base.StartPhase();
    }
}
