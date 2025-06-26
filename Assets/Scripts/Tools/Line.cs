using UnityEngine;

public class Line : Tool
{
    Vector2Int point0 = -Vector2Int.one;

    public override bool OnUpdate(Vector2Int point, Vector2Int lastPoint, bool usePressed) { return false; }

    public override void OnUse(Vector2Int point, Vector2Int lastPoint) 
    { 
        if (point0 != -Vector2.one)
        {
            var board = DrawingManager.instance.GetBoard();

            board.DrawBetween(point0, point, DrawingManager.instance.GetColor());

            point0 = -Vector2Int.one;
            board.UpdateHistory();
        } else
        {
            point0 = point;
        }
    }

    public override void OnBoardChanged() 
    {
        point0 = -Vector2Int.one;
    }
}
