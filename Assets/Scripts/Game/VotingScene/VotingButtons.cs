using UnityEngine;
using UnityEngine.UI;

public class VotingButtons : MonoBehaviour
{
    [SerializeField] Button[] buttons;

    public VoteType currentVote = VoteType.None;

    public bool invested = false;

    private void OnEnable()
    {
        foreach (var button in buttons) 
        {
            button.interactable = false;
        }
    }

    public void ButtonPressed(int voteIndex)
    {
        VoteType vote = (VoteType)voteIndex;

        if (vote == VoteType.None)                  // meme buddy button
        {
            invested = true;
            buttons[0].gameObject.SetActive(false);
        } else
        {
            currentVote = vote;

            for (int i = 0; i < buttons.Length; i++) 
            {
                if (i == voteIndex) 
                {
                    buttons[i].interactable = false;
                } else if (i != 0)
                {
                    buttons[i].interactable = true;
                }
            }
        }
    }

    public void DisableButtons()
    {
        foreach (var button in buttons)
        {
            button.interactable = false;
        }
    }

    public void ResetVote()
    {
        currentVote = VoteType.None;

        foreach (var button in buttons)
        {
            button.interactable = true;
        }

        invested = false;
    }
}
