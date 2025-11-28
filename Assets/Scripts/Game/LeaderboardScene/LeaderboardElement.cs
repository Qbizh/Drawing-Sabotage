using UnityEngine;
using TMPro;

public class LeaderboardElement : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text scoreText;

    public void Setup(string name, int score)
    {
        nameText.text = name;
        scoreText.text = score.ToString();
    }
}
