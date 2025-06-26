using UnityEngine;

public class Erase : Tool
{
    bool lastPressed = false;

    public override bool OnUpdate(Vector2Int point, Vector2Int lastPoint, bool usePressed)
    {
        DrawingBoard board = DrawingManager.instance.GetBoard();

        if (usePressed)
        {
            board.DrawPoint(point, Color.white);

            if (lastPoint.x != -1)
            {
                board.DrawBetween(lastPoint, point, Color.white);
            }
        } else if (usePressed != lastPressed)
        {
            board.UpdateHistory();
        }

        lastPressed = usePressed;

        return usePressed;
    }

    public override void OnUse(Vector2Int point, Vector2Int lastPoint) { }

    public override void OnBoardChanged() { }
}
