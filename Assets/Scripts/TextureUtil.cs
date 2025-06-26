using UnityEngine;

public static class TextureUtil
{
    

    public static bool IsSameColor(Color color1, Color color2)
    {
        return (color1.a == 0 && color2.a == 0) || (Mathf.Abs(color1.r - color2.r) <= TextureConstants.colorThreshold && Mathf.Abs(color1.g - color2.g) <= TextureConstants.colorThreshold 
            && Mathf.Abs(color1.b - color2.b) <= TextureConstants.colorThreshold && Mathf.Abs(color1.a - color2.a) <= TextureConstants.colorThreshold);
    }
}
