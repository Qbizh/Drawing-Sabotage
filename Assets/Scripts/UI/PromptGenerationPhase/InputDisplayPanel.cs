using TMPro;
using UnityEngine;
using System.Collections.Generic;
using FishNet;
using System.Linq;
using UnityEngine.InputSystem;

public class InputDisplayPanel : MonoBehaviour
{
    Animator scrollAnimator;
    TMP_Text[] inputTexts;

    private string tag;
    private string input;

    public bool doneAnimation = true;

    private void Awake()
    {
        scrollAnimator = GetComponentInChildren<Animator>();

        inputTexts = scrollAnimator.gameObject.GetComponentsInChildren<TMP_Text>();
    }

    public void Setup(string newTag, string newInput)
    {
        tag = newTag;
        input = newInput;

        GamePhaseManager.instance.gameDataHolder.promptInputsRecieved += OnPromptInputsRecieved;
        GamePhaseManager.instance.gameDataHolder.RequestPromptInputs(InstanceFinder.ClientManager.Connection);
    }

    public void StartAnimation()
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

    }
}
