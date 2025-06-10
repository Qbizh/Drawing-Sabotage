using UnityEngine;

public abstract class Tool
{
    public abstract void OnUse(Vector2Int point, Vector2Int lastPoint);

    public abstract void OnUpdate(Vector2Int point, Vector2Int lastPoint, bool usePressed);

    public abstract void OnBoardChanged();
}
