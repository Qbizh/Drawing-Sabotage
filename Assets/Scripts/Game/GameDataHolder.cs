using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class GameDataHolder : NetworkBehaviour
{
    [SerializeField] PromptFormatList formatList;

    List<string> unfulfilledFormats = new List<string>();
    List<string> fulfilledFormats = new List<string>();

    public readonly SyncVar<string> currentPrompt = new SyncVar<string>();

    Dictionary<string, List<string>> promptInputs = new Dictionary<string, List<string>>();

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
        var newFulfilledFormats = fulfilledFormats;
        var newUnfulfilledFormats = unfulfilledFormats;

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
            if (IsFormatFulfilled(format, promptInputs))
            {
                newFulfilledFormats.Add(format);
                newUnfulfilledFormats.Remove(format);
            }
        }

        fulfilledFormats = newFulfilledFormats;
        unfulfilledFormats = newUnfulfilledFormats;

        return fulfilledFormats.Count > 0;
    }

    [Server]
    public void GeneratePrompt()
    {
        string prompt = fulfilledFormats[Random.Range(0, fulfilledFormats.Count)];

        fulfilledFormats.Remove(prompt);

        MatchCollection matches = Regex.Matches(prompt, @"\[(.*?)\]");      // the second parameter basically says grab anything in brackets

        foreach (Match match in matches)
        {
            string tag = match.Value;

            List<string> inputs;
            
            if (promptInputs.TryGetValue(tag, out inputs))
            {
                string input = inputs[Random.Range(0, inputs.Count)];
                inputs.Remove(input);

                prompt.Replace(tag, input);
            } else
            {
                Debug.LogError("NO INPUT FOUND FOR TAG: " + tag);
            }
        }

        currentPrompt.Value = prompt;
    }

    [Server]
    public bool AddPromptInput(string tag, string input)
    {
        promptInputs[tag].Add(input);

        return AssessFormats();
    }

}
