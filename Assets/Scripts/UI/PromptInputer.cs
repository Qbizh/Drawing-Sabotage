using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PromptInputer : MonoBehaviour
{
    [SerializeField] PromptInputPhaseHandler promptPhaseHandler;

    [SerializeField] TMP_InputField promptInputField;
    [SerializeField] TMP_Text tagDisplay;
    [SerializeField] Image tagBackground;

    Dictionary<string, Color> tagColors = new Dictionary<string, Color>();

    Queue<string> tagsQueue;

    private void OnEnable()
    {
        PhaseHandler.phaseSetUp += OnPhaseSetUp;
    }

    private void OnDisable()
    {
        PhaseHandler.phaseSetUp -= OnPhaseSetUp;
    }

    private void OnPhaseSetUp(bool asServer)
    {
        if (!asServer)
        {
            tagsQueue = new Queue<string>(promptPhaseHandler.tags);
            Debug.Log(tagsQueue.Count);
            foreach (string tag in tagsQueue)
            {
                tagColors[tag] = Color.HSVToRGB(Random.Range(0f, 1f), 0.57f, 0.75f);
                
            }

            GetNextTag();
        }
    }

    private void GetNextTag()
    {
        string tag = tagsQueue.Peek();
        
        tagBackground.color = tagColors[tag];

        tag = tag.Remove(0, 1);
        tag = tag.Remove(tag.Length - 1, 1);

        tagDisplay.text = tag;

    }

    public void OnPromptSubmit()
    {
        string promptInput = promptInputField.text;

        if (PhaseHandler.phaseActive && !string.IsNullOrEmpty(promptInput)) 
        {

            promptInputField.text = "";

            string tag = tagsQueue.Dequeue();

            promptPhaseHandler.AddPromptInput(tag, promptInput);

            tagsQueue.Enqueue(tag);

            GetNextTag();
        } 
    }
}