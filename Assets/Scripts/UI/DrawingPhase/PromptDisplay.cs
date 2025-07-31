using UnityEngine;
using TMPro;

public class PromptDisplay : MonoBehaviour
{
    private void OnEnable()
    {
        PhaseHandler.phaseSetUp += OnPhaseSetUp;
    }

    private void OnDisable()
    {
        PhaseHandler.phaseSetUp -= OnPhaseSetUp;
        //GamePhaseManager.instance.gameDataHolder.currentPrompt.OnChange -= OnPromptChange;
    }

    private void OnPhaseSetUp(bool asServer)
    {
        if (!asServer)
        {
            GetComponent<TMP_Text>().text = GamePhaseManager.instance.gameDataHolder.currentPrompt.Value;

            //GamePhaseManager.instance.gameDataHolder.currentPrompt.OnChange += OnPromptChange;                  // just in case client doesn't recieve the prompt before they load into drawing phase - super edge case
        }
    }

    /*private void OnPromptChange(string old, string next, bool asServer)
    {
        if (!asServer)
        {
            GetComponent<TMP_Text>().text = next;
        }
    }*/
}
