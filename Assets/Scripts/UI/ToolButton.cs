using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ToolButton : Button
{
    public int toolIndex = 0;

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        
        ToolButton[] allToolButtons = FindObjectsByType<ToolButton>(FindObjectsSortMode.None);
        foreach (ToolButton toolButton in allToolButtons)
        {
            if (toolButton != this && toolButton.currentSelectionState == SelectionState.Selected)
            {
                toolButton.OnDeselect(null);
            }
        }
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        if (eventData == null)
        {
            base.OnDeselect(eventData);
        }
    }

    public void SetTool(int i)
    {
        toolIndex = i;
        DrawingManager.instance.SetTool(toolIndex);
    }
}
