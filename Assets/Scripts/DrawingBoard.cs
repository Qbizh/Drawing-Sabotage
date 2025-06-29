using UnityEngine;
using System.IO;
using System;

public class DrawingBoard : MonoBehaviour
{
    public Texture2D texture;
    Texture2D[] textureHistory;

    int currentHistoryIndex = 0;

    public Color defaultBackground;

    float boardToTextureRatio = 0;

    Vector2 actualBoardSize;

    int texWidth;
    int texHeight;

    public event Action textureChanged;

    public void Start()
    {
        var sprite = GetComponent<SpriteRenderer>().sprite;

        texWidth = sprite.texture.width;
        texHeight = sprite.texture.height;

        texture = new Texture2D(texWidth, texHeight);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;


        Color[] colors = new Color[texWidth * texHeight];
        Array.Fill<Color>(colors, defaultBackground);

        texture.SetPixels(colors);
        ApplyChanges();

        var newSprite = Sprite.Create(texture, new Rect(0,0, texWidth, texHeight), Vector2.one * 0.5f);
        GetComponent<SpriteRenderer>().sprite = newSprite;

        actualBoardSize = new Vector2(texWidth * transform.lossyScale.x / newSprite.pixelsPerUnit, texHeight * transform.lossyScale.y / newSprite.pixelsPerUnit);
        boardToTextureRatio = texWidth / actualBoardSize.x;

        ClearHistory();
    }


    public Vector2Int GetPointOnBoard(Vector2 position)
    {
        Vector2 boardPos = position - (Vector2)transform.position + (Vector2)actualBoardSize / 2;

        Vector2Int point = new Vector2Int(Mathf.RoundToInt(boardPos.x * boardToTextureRatio), Mathf.RoundToInt(boardPos.y * boardToTextureRatio));
        
        return point;
    }

    public void DrawPoint(Vector2Int point, Color color, float strokeSize)
    {
        for (int y = (int)(point.y - strokeSize / 2); y <= (int)(point.y + strokeSize / 2); y++)
        {
            for (int x = (int)(point.x - strokeSize / 2); x <= (int)(point.x + strokeSize / 2); x++)
            {
                float relX = x - point.x;
                float relY = y - point.y;

                float dist = Mathf.RoundToInt(Mathf.Sqrt(relX * relX + relY * relY));

                if (dist <= strokeSize / 2)
                {
                    SetPixel(new Vector2Int(x, y), color);
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
                    DrawPoint(new Vector2Int(x, y), color, DrawingManager.instance.GetStrokeSize());
                }

            }
        }
    }

    public void StampTexture(Texture2D stamp, float scale, Vector2Int point)
    {
        //Color[] pixels = texture.GetPixels();

        int targetResolution = Mathf.FloorToInt(stamp.width * scale);

        for (int y = point.y - targetResolution / 2; y <= point.y + targetResolution / 2; y++)
        {
            if (y >= 0 || y <= texture.height)
            {
                for (int x = point.x - targetResolution / 2; y <= point.x + targetResolution / 2; x++)
                {
                    if (x >= 0 || x <= texture.width)
                    {
                        texture.SetPixel(x,y, stamp.GetPixel(Mathf.FloorToInt(x * 1 / scale), Mathf.FloorToInt(y * 1 / scale)));
                    }
                }
            }
        }

        //texture.SetPixels(pixels);
    }

    public void ApplyChanges()
    {
        texture.Apply();
        textureChanged?.Invoke();
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

    public void ClearHistory()
    {
        currentHistoryIndex = 0;

        textureHistory = new Texture2D[DrawingManager.instance.UndoHistoryLength];

        Texture2D newSave = new Texture2D(texWidth, texHeight);
        newSave.CopyPixels(texture);

        textureHistory[0] = newSave;
    }

    public void UnDo()
    {
        if (currentHistoryIndex + 1 < textureHistory.Length && textureHistory[currentHistoryIndex + 1] != null)
        {
            currentHistoryIndex += 1;

            texture.CopyPixels(textureHistory[currentHistoryIndex]);
            ApplyChanges();
        }
    }

    public void ReDo()
    {
        if (currentHistoryIndex - 1 >= 0)
        {
            currentHistoryIndex -= 1;

            texture.CopyPixels(textureHistory[currentHistoryIndex]);
            ApplyChanges();
        }
    }

    public Color GetPixel(Vector2Int point)
    {
        return texture.GetPixel(point.x, point.y);
    }

    public void SetPixel(Vector2Int point, Color color)
    {
        if (color.a == 0)
        {
            color = defaultBackground;
        }

        texture.SetPixel(point.x, point.y, color);

    }

    public static DrawingBoard GetMainBoard()
    {
        var mainBoard = GameObject.FindGameObjectWithTag("MainBoard");
        
        return mainBoard.GetComponent<DrawingBoard>();
    }

    [ContextMenu("Save Board To PNG")]
    public void SaveBoardToPNG()
    {
        var texBytes = texture.EncodeToPNG();

        var directory = Application.dataPath + "/SavedBoards/";

        if (!Directory.Exists(directory)) { 

            Directory.CreateDirectory(directory);
        }

        int count = 0;

        var imgDir = directory + "Board";


        while (File.Exists(imgDir + count + ".png"))
        {
            count++;
        }

        imgDir = imgDir + count + ".png";

        File.WriteAllBytes(imgDir, texBytes);
    }
}
