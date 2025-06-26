using UnityEngine;
using System.Collections.Generic;

public class Fill : Tool
{
    public override bool OnUpdate(Vector2Int point, Vector2Int lastPoint, bool usePressed) { return false; }

    public override void OnUse(Vector2Int point, Vector2Int lastPoint) 
    {
        DrawingBoard board = DrawingManager.instance.GetBoard();

        Color originalColor = board.GetPixel(point);
        Color targetColor = DrawingManager.instance.GetColor();
 
        if (TextureUtil.IsSameColor(originalColor, targetColor)) return;

        Queue<Vector2Int> fillQueue = new Queue<Vector2Int>();

        fillQueue.Enqueue(point);

        while (fillQueue.Count > 0)
        {
            var currentPoint = fillQueue.Dequeue();

            if (board.GetPixel(currentPoint) == originalColor)
            {
                board.SetPixel(currentPoint, targetColor);

                fillQueue.Enqueue(new Vector2Int(currentPoint.x, currentPoint.y + 1)); // up
                fillQueue.Enqueue(new Vector2Int(currentPoint.x + 1, currentPoint.y)); // right
                fillQueue.Enqueue(new Vector2Int(currentPoint.x, currentPoint.y - 1)); // down
                fillQueue.Enqueue(new Vector2Int(currentPoint.x - 1, currentPoint.y)); // left
            }
        }

        board.UpdateHistory();
    }

    public override void OnBoardChanged() { }
}
