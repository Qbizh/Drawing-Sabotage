using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class GameDataHolder : MonoBehaviour
{
    [SerializeField] PromptFormatList formatList;

    List<string> formats = new List<string>();

    private void OnEnable()
    {
        formats = formatList.formats;
    }

    public HashSet<string> GetFormatTags()
    {
        HashSet<string> tags = new HashSet<string>();

        foreach (var format in formats)
        {
            MatchCollection matches = Regex.Matches(format, @"\[(.*?)\]");      // the second parameter basically says grab anything in brackets

            foreach (Match match in matches)
            {
                tags.Add(match.Value);
            }
        }

        return tags;
    }
}
