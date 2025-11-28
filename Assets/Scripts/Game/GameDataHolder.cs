using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Linq;
using FishNet.Connection;
using System;

public class GameDataHolder : NetworkBehaviour
{
    [SerializeField] PromptFormatList formatList;

    List<string> unfulfilledFormats = new List<string>();
    List<string> fulfilledFormats = new List<string>();

    public readonly SyncVar<string> currentPrompt = new SyncVar<string>();
    public readonly SyncDictionary<NetworkConnection, byte[]> playerDrawings = new SyncDictionary<NetworkConnection, byte[]>();

    Dictionary<string, List<string>> promptInputs = new Dictionary<string, List<string>>();
    public event Action<Dictionary<string, List<string>>> promptInputsRecieved;

    public readonly SyncDictionary<NetworkConnection, int> gameScores = new SyncDictionary<NetworkConnection, int>();

    public struct PromptData 
    { 
        public string prompt;
        public string format;
        public List<string> tags;
        public List<string> inputs;

        public PromptData (string prompt, string format, List<string> inputs, List<string> tags)
        {
            this.prompt = prompt;
            this.format = format;
            this.inputs = inputs;
            this.tags = tags;
        }
    }


    private void OnEnable()
    {
        unfulfilledFormats = formatList.formats;
    }

    public HashSet<string> GetUnfulfilledFormatTags()
    {
        HashSet<string> tags = new HashSet<string>();

        foreach (var format in unfulfilledFormats)
        {
            MatchCollection matches = Regex.Matches(format, @"\[(.*?)\]");      // the second parameter basically says grab anything in brackets

            foreach (Match match in matches)
            {
                tags.Add(match.Value);
            }
        }

        return tags;
    }

    public bool IsFormatFulfilled(string format, Dictionary<string, List<string>> inputs)
    {
        MatchCollection matches = Regex.Matches(format, @"\[(.*?)\]");

        foreach (Match match in matches)
        {
            string tag = match.Value;

            if (!inputs.ContainsKey(tag) || inputs[tag].Count == 0)
            {
                return false;
            }
        }

        return true;
    }

    public bool AssessFormats()
    {

        var newFulfilledFormats = fulfilledFormats.ToList();
        var newUnfulfilledFormats = unfulfilledFormats.ToList();

        foreach (var format in unfulfilledFormats)
        {
            if (IsFormatFulfilled(format, promptInputs))
            {
                newFulfilledFormats.Add(format);
                newUnfulfilledFormats.Remove(format);
            }
        }

        foreach (var format in fulfilledFormats)
        {
            if (!IsFormatFulfilled(format, promptInputs))
            {
                newFulfilledFormats.Remove(format);
                newUnfulfilledFormats.Add(format);
            }
        }
        

        fulfilledFormats = newFulfilledFormats;
        unfulfilledFormats = newUnfulfilledFormats;

        return fulfilledFormats.Count > 0;
    }

    [Server]
    public PromptData GeneratePrompt()
    {
        int rand = UnityEngine.Random.Range(0, fulfilledFormats.Count);

        string format = fulfilledFormats[rand];

        string prompt = format;

        var promptData = new PromptData(null, format, null, null);
        var usedInputs = new List<string>();
        var usedTags = new List<string>();

        MatchCollection matches = Regex.Matches(prompt, @"\[(.*?)\]");      // the second parameter basically says grab anything in brackets

        foreach (Match match in matches)
        {
            string tag = match.Value;

            List<string> inputs;
            
            if (promptInputs.TryGetValue(tag, out inputs))
            {
                string input = inputs[UnityEngine.Random.Range(0, inputs.Count)];

                usedTags.Add(tag);
                usedInputs.Add(input);

                prompt = prompt.Replace(tag, input);
            } else
            {
                Debug.LogError("NO INPUT FOUND FOR TAG: " + tag);
            }
        }

        promptData.prompt = prompt;
        promptData.inputs = usedInputs;
        promptData.tags = usedTags;

        return promptData;
    }

    [Server]
    public void SetPrompt(PromptData promptData)
    {
        fulfilledFormats.Remove(promptData.format);

        for (int i = 0; i < promptData.inputs.Count; i++)
        {
            promptInputs[promptData.tags[i]].Remove(promptData.inputs[i]);    // remove input from tag
        }

        currentPrompt.Value = promptData.prompt;
    }

    [Server]
    public bool AddPromptInput(string tag, string input)
    {
        if (!promptInputs.TryAdd(tag, new List<string>() {input})) 
        {
            promptInputs[tag].Add(input);
        }

        return AssessFormats();
    }

    [ServerRpc(RequireOwnership =false)]
    public void RequestPromptInputs(NetworkConnection conn)
    {
        
        RecievePromptInputs(conn, promptInputs);
    }

    [TargetRpc]
    private void RecievePromptInputs(NetworkConnection conn, Dictionary<string, List<string>> inputs)
    {
        promptInputsRecieved?.Invoke(inputs);
    }

    [Server]
    public void SetPlayerDrawings(Dictionary<NetworkConnection, byte[]> drawings)
    {
        playerDrawings.Clear();
        
        foreach (var kvp in drawings)
        {
            playerDrawings.Add(kvp.Key, kvp.Value);
        }
    }

    [Server]
    public void AddRoundScores(Dictionary<NetworkConnection, int> scores)
    {
        foreach(var player in scores.Keys)
        {
            AddScore(player, scores[player]);
        }
    }

    private void AddScore(NetworkConnection player, int add)
    {
        int currentScore;

        if (gameScores.TryGetValue(player, out currentScore))
        {
            gameScores[player] = currentScore + add;
        }
        else
        {
            gameScores.Add(player, add);
        }
    }

}
