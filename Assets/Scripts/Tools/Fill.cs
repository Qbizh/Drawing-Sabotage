using UnityEngine;
using System.Collections.Generic;

public class Fill : Tool
{
    float colorThreshold = 0.005f;

    public override void OnUpdate(Vector2Int point, Vector2Int lastPoint, bool usePressed) {}

    public override void OnUse(Vector2Int point, Vector2Int lastPoint) 
    {
        DrawingBoard board = DrawingManager.instance.GetBoard();

        Color originalColor = board.GetPixel(point);
        Color targetColor = DrawingManager.instance.GetColor();
 
        if (IsSameColor(originalColor, targetColor)) return;

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

    bool IsSameColor(Color color1, Color color2)
    {
        return Mathf.Abs(color1.r - color2.r) <= colorThreshold && Mathf.Abs(color1.g - color2.g) <= colorThreshold && Mathf.Abs(color1.b - color2.b) <= colorThreshold;
    }

    public override void OnBoardChanged() { }
}
