using UnityEngine;
using System.Collections.Generic;

public static class TextureUtil
{
    

    public static bool IsSameColor(Color color1, Color color2)
    {
        return (color1.a == 0 && color2.a == 0) || (Mathf.Abs(color1.r - color2.r) <= TextureConstants.colorThreshold && Mathf.Abs(color1.g - color2.g) <= TextureConstants.colorThreshold 
            && Mathf.Abs(color1.b - color2.b) <= TextureConstants.colorThreshold && Mathf.Abs(color1.a - color2.a) <= TextureConstants.colorThreshold);
    }

    public static List<Vector2> GetTextureEdges(Texture2D texture)
    {
        List<Vector2> edgePoints = new List<Vector2>();

        for (int y = 0; y <= texture.width; y++)
        {
            for (int x = 0; x <= texture.height; x++)
            {
                if (texture.GetPixel(x, y).a != 0)
                {
                    bool up = y == texture.height || texture.GetPixel(x, y + 1).a == 0; 
                    bool right = x == texture.width || texture.GetPixel(x + 1, y).a == 0;
                    bool down = y == 0 || texture.GetPixel(x, y - 1).a == 0;
                    bool left = x == 0 || texture.GetPixel(x - 1, y).a == 0;

                    if (up || right || down || left)
                    {
                        edgePoints.Add(new Vector2(x, y));
                    }
                }
            }
        }

        return edgePoints;
    }
}
