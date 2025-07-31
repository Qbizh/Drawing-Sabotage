using TMPro;
using UnityEngine;
using System.Collections.Generic;
using FishNet;
using UnityEngine.UI;

public class InputDisplayPanel : MonoBehaviour
{
    [SerializeField] private GameObject scrollingList;
    Image backgroundImage;

    Animator scrollAnimator;
    TMP_Text[] inputTexts;

    private string tag;
    private string input;

    public bool doneAnimation = false;

    private void Awake()
    {
        scrollAnimator = GetComponentInChildren<Animator>();

        inputTexts = scrollAnimator.gameObject.GetComponentsInChildren<TMP_Text>();

        backgroundImage = GetComponent<Image>();

        scrollingList.SetActive(false);
        backgroundImage.enabled = false;
    }

    public void Setup(string newTag, string newInput)
    {
        tag = newTag;
        input = newInput;

        GamePhaseManager.instance.gameDataHolder.promptInputsRecieved += OnPromptInputsRecieved;
        GamePhaseManager.instance.gameDataHolder.RequestPromptInputs(InstanceFinder.ClientManager.Connection);
    }

    private void StartAnimation()
    {
        doneAnimation = false;
        scrollAnimator.SetTrigger("Scroll");
    }

    public void OnAnimationEnd()
    {
        doneAnimation = true;
    }

    private void OnPromptInputsRecieved(Dictionary<string, List<string>> inputs)
    {
        GamePhaseManager.instance.gameDataHolder.promptInputsRecieved -= OnPromptInputsRecieved;

        List<string> tagInputs;

        if (inputs.TryGetValue(tag, out tagInputs)) 
        {
            var inputQueue = new Queue<string>(tagInputs);

            for (int i = 0; i < inputTexts.Length; i++) 
            {
                if (i == inputTexts.Length - 1)
                {
                    inputTexts[i].text = input;
                } else
                {
                    var input = inputQueue.Dequeue();
                    inputTexts[i].text = input;

                    inputQueue.Enqueue(input);
                }
            }
        }

        backgroundImage.color = Color.HSVToRGB(Random.Range(0f, 1f), 0.57f, 0.75f);

        scrollingList.SetActive(true);
        backgroundImage.enabled = true;

        Invoke("StartAnimation", Random.Range(0f, 0.8f));
    }
}
