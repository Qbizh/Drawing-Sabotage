using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    [SerializeField] Image selectedColorDisplay;

    public void SetColor()
    {
        GameObject clickedObj = EventSystem.current.currentSelectedGameObject;
        
        DrawingManager.instance.SetColor(clickedObj.GetComponent<Button>().colors.normalColor);

        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        selectedColorDisplay.color = DrawingManager.instance.GetColor();
    }

    private void Update()
    {
        UpdateDisplay();
    }
}
