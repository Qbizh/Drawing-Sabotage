using UnityEngine;

public class EyeDropper : Tool
{
    public override bool OnUpdate(Vector2Int point, Vector2Int lastPoint, bool usePressed) { return false; }

    public override void OnUse(Vector2Int point, Vector2Int lastPoint) 
    {
        DrawingManager.instance.SetColor(DrawingManager.instance.GetBoard().GetPixel(point));
    }

    public override void OnBoardChanged() {}
}
