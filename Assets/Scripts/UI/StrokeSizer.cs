using UnityEngine;

public class StrokeSizer : MonoBehaviour
{
    public void SetStroke(float stroke)
    {
        DrawingManager.instance.SetStrokeSize(stroke);
    }
}
