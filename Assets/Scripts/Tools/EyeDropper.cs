using UnityEngine;

public class EyeDropper : Tool
{
    public override void OnUpdate(Vector2Int point, Vector2Int lastPoint, bool usePressed) {}

    public override void OnUse(Vector2Int point, Vector2Int lastPoint) 
    {
        DrawingManager.instance.SetColor(DrawingManager.instance.GetBoard().GetPixel(point));
    }

    public override void OnBoardChanged() {}
}
