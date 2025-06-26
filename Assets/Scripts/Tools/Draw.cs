using UnityEngine;

public class Draw : Tool
{
    bool lastPressed = false;

    public override bool OnUpdate(Vector2Int point, Vector2Int lastPoint, bool usePressed)
    {
        Color color = DrawingManager.instance.GetColor();
        DrawingBoard board = DrawingManager.instance.GetBoard();

        if (usePressed)
        {
            board.DrawPoint(point, color);

            if (lastPoint.x != -1)
            {
                board.DrawBetween(lastPoint, point, color);
            }
        }
        else if (usePressed != lastPressed)
        {
            board.UpdateHistory();
        }

        lastPressed = usePressed;

        return usePressed;
    }

    public override void OnUse(Vector2Int point, Vector2Int lastPoint) {}

    public override void OnBoardChanged() { }
}
