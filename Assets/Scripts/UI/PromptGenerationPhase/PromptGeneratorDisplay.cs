using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using TMPro;
using UnityEngine;
using WebSocketSharp;

public class PromptGeneratorDisplay : NetworkBehaviour
{

    [SerializeField] GameObject promptLinePrefab;
    [SerializeField] GameObject inputPanelPrefab;
    [SerializeField] GameObject promptTextPrefab;

    Dictionary<GameObject, List<GameObject>> promptLines = new Dictionary<GameObject, List<GameObject>>();

    int maxLineLength = 3;

    private void OnEnable()
    {
        PhaseHandler.phaseStart += OnPhaseStart;
    }

    private void OnDisable()
    {
        PhaseHandler.phaseStart -= OnPhaseStart;
        InputManager.instance.onReRoll -= OnReRoll;
    }

    private void OnPhaseStart(bool asServer)
    {
        if (asServer)
        {
            InputManager.instance.onReRoll += OnReRoll;

            GeneratePrompt();
        }
    }

    private void OnPromptDataChanged(GameDataHolder.PromptData old, GameDataHolder.PromptData next, bool asServer)
    {
        if (!asServer)
        {
            SetUpPromptDisplay(next);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void GeneratePrompt()
    {


        foreach (GameObject line in promptLines.Keys)
        {
            Destroy(line);
        }

        promptLines.Clear();

        GameDataHolder.PromptData promptData = GamePhaseManager.instance.gameDataHolder.GeneratePrompt();

        SetUpPromptDisplay(promptData);
    }

    [ObserversRpc]
    private void SetUpPromptDisplay(GameDataHolder.PromptData promptData)
    {
        string[] splitFormat = Regex.Split(promptData.format, @"(\[.*?\])");

        GameObject currentLine = null;

        int tagIndex = 0;

        foreach (string str in splitFormat)
        {
            if (str.IsNullOrEmpty() || str == " ") continue;

            if (currentLine == null || promptLines[currentLine].Count >= maxLineLength)
            {
                currentLine = Instantiate(promptLinePrefab, transform);
                promptLines.Add(currentLine, new List<GameObject>());
            }

            GameObject obj;

            if (str.Contains("[") && str.Contains("]"))             // is a tag
            {
                
                obj = Instantiate(inputPanelPrefab, currentLine.transform);
                obj.GetComponent<InputDisplayPanel>().Setup(str, promptData.inputs[tagIndex]);
                obj.GetComponent<InputDisplayPanel>().StartAnimation();

                promptLines[currentLine].Add(gameObject);

                tagIndex++;
            } else
            {
                obj = Instantiate(promptTextPrefab, currentLine.transform);

                string text = str[0] == ' ' ? str.Remove(0, 1) : str;                               // remove spaces on either ends
                text = text[text.Length - 1] == ' ' ? text.Remove(text.Length - 1, 1) : text;

                obj.GetComponent<TMP_Text>().text = text;
            }

            promptLines[currentLine].Add(obj);
        }
    }

    private void OnReRoll()
    {
        if (IsHostInitialized)
        {
            bool ready = true;

            foreach (var line in promptLines.Values) 
            {
                foreach (var obj in line) 
                {
                    InputDisplayPanel inputDisplayPanel;

                    if (obj.TryGetComponent<InputDisplayPanel>(out inputDisplayPanel))
                    {
                        if (!inputDisplayPanel.doneAnimation)
                        {
                            ready = false;
                            break;
                        }
                    }
                }
            }

            if (ready)
            {
                GeneratePrompt();
            }
        }
    }

    private bool IsGenerationAnimationDone()
    {
        return false;
    }
}