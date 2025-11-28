using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerResultsDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] Image drawingDispay;


    public void SetUp(string name, int score, Sprite sprite)
    {
        nameText.text = name;
        scoreText.text = score.ToString();
        drawingDispay.sprite = sprite;
    }
}
