using UnityEngine;
using System.Collections;
using System;

public class DrawingBoard : MonoBehaviour
{
    public Texture2D texture;
    Texture2D[] textureHistory;

    [SerializeField] int currentHistoryIndex = 0;

    float boardToTextureRatio = 0;

    Vector2 actualBoardSize;

    int texWidth;
    int texHeight;

    void Start()
    {
        var sprite = GetComponent<SpriteRenderer>().sprite;

        texWidth = sprite.texture.width;
        texHeight = sprite.texture.height;

        texture = new Texture2D(texWidth, texHeight);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        textureHistory = new Texture2D[DrawingManager.instance.UndoHistoryLength];

        Color[] colors = new Color[texWidth * texHeight];
        Array.Fill<Color>(colors, Color.white);

        texture.SetPixels(colors);
        texture.Apply();

        var newSprite = Sprite.Create(texture, new Rect(0,0, texWidth, texHeight), Vector2.one * 0.5f);
        GetComponent<SpriteRenderer>().sprite = newSprite;

        actualBoardSize = new Vector2(texWidth * transform.lossyScale.x / newSprite.pixelsPerUnit, texHeight * transform.lossyScale.y / newSprite.pixelsPerUnit);
        boardToTextureRatio = texWidth / actualBoardSize.x;

        Texture2D newSave = new Texture2D(texWidth, texHeight);
        newSave.CopyPixels(texture);

        textureHistory[0] = newSave;
    }


    public Vector2Int GetPointOnBoard(Vector2 mousePos)
    {
        Vector2 boardPos = mousePos - (Vector2)transform.position + (Vector2)actualBoardSize / 2;

        Vector2Int point = new Vector2Int(Mathf.RoundToInt(boardPos.x * boardToTextureRatio), Mathf.RoundToInt(boardPos.y * boardToTextureRatio));
        
        return point;
    }

    public void DrawPoint(Vector2Int point, Color color)
    {
        float strokeSize = DrawingManager.instance.GetStrokeSize();

        for (int y = (int)(point.y - strokeSize / 2); y <= (int)(point.y + strokeSize / 2); y++)
        {
            for (int x = (int)(point.x - strokeSize / 2); x <= (int)(point.x + strokeSize / 2); x++)
            {
                float relX = x - point.x;
                float relY = y - point.y;

                float dist = Mathf.RoundToInt(Mathf.Sqrt(relX * relX + relY * relY));

                if (dist <= strokeSize / 2)
                {
                    texture.SetPixel(x, y, color);
                }
            }
        }
    }

    public void DrawBetween(Vector2Int start, Vector2Int end, Color color)
    {
        int minX = Mathf.Min(end.x, start.x);
        int maxX = Mathf.Max(start.x, end.x);

        int minY = Mathf.Min(end.y, start.y);
        int maxY = Mathf.Max(start.y, end.y);

        float deltaX = end.x - start.x;
        float deltaY = end.y - start.y;
        
        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                int predictedY = Mathf.RoundToInt(deltaY * ((x - start.x) / deltaX) + start.y);
                int predictedX  = Mathf.RoundToInt(deltaX * ((y - start.y) / deltaY) + start.x);

                if (predictedY == y || predictedX == x)
                {
                    DrawPoint(new Vector2Int(x, y), color);
                }

            }
        }
    }

    public void ApplyChanges()
    {
        texture.Apply();
    }

    public void UpdateHistory()
    {
        Texture2D[] updatedHistory = new Texture2D[textureHistory.Length];

        Texture2D newSave = new Texture2D(texWidth, texHeight);
        newSave.CopyPixels(texture);

        updatedHistory[0] = newSave;
       
        for (int i = currentHistoryIndex; i < textureHistory.Length; i++)
        {
            if (textureHistory[i] != null && i + 1 - currentHistoryIndex < textureHistory.Length)
            {
                updatedHistory[i + 1 - currentHistoryIndex] = textureHistory[i];
            } else
            {
                break;
            }
        }

        currentHistoryIndex = 0;

        textureHistory = updatedHistory;
    }

    public void UnDo()
    {
        if (currentHistoryIndex + 1 < textureHistory.Length && textureHistory[currentHistoryIndex + 1] != null)
        {
            currentHistoryIndex += 1;

            texture.CopyPixels(textureHistory[currentHistoryIndex]);
            texture.Apply();
        }
    }

    public void ReDo()
    {
        

        if (currentHistoryIndex - 1 >= 0)
        {
            currentHistoryIndex -= 1;

            texture.CopyPixels(textureHistory[currentHistoryIndex]);
            texture.Apply();
        }
    }

    public Color GetPixel(Vector2Int point)
    {
        return texture.GetPixel(point.x, point.y);
    }

    public void SetPixel(Vector2Int point, Color color)
    {
        texture.SetPixel(point.x, point.y, color);
    }
}
