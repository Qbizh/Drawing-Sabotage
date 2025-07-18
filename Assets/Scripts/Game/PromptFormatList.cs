using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Prompt Formats", menuName = "ScriptableObject/Prompt Formats")]
public class PromptFormatList : ScriptableObject
{
    public List<string> formats = new List<string>();


}
