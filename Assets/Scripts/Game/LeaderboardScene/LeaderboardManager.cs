using UnityEngine;
using FishNet.Object;
using System.Linq;

public class LeaderboardManager : NetworkBehaviour
{
    [SerializeField] Transform leaderboard;

    [SerializeField] GameObject leaderboardElement;

    private void OnEnable()
    {
        PhaseHandler.phaseSetUp += UpdateLeaderboard;
    }

    private void OnDisable()
    {
        PhaseHandler.phaseSetUp -= UpdateLeaderboard;
    }

    public void UpdateLeaderboard(bool asServer)
    {
        if (!asServer)
        {
            foreach (Transform obj in leaderboard)
            {
                Destroy(obj);
            }

            var players = LobbyManager.instance.players;
            var scores = GamePhaseManager.instance.gameDataHolder.gameScores;

            Debug.Log(scores.Count);

            foreach (var kvp in scores.OrderByDescending(x => x.Value))
            {
                Debug.Log(kvp.Value);

                int score = kvp.Value;
                string name = players[kvp.Key].name;

                var element = Instantiate(leaderboardElement);
                element.GetComponent<LeaderboardElement>().Setup(name, score);
                element.transform.SetParent(leaderboard);
            }
        }
    }
}
