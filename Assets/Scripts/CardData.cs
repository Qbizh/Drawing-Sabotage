using UnityEngine;

[CreateAssetMenu(fileName = "Card Data", menuName = "ScriptableObject/Card Data")]
public class CardData : ScriptableObject
{
    public string cardName;
    public string description;

    public Texture2D texture;

    public GameObject prefab;

    public Sprite GenerateSprite(int width, int height)
    {
        var newTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        newTexture.filterMode = FilterMode.Point;
        newTexture.wrapMode = TextureWrapMode.Clamp;

        newTexture.CopyPixels(texture);
        newTexture.Apply();

        return Sprite.Create(newTexture, new Rect(0, 0, width, height), Vector2.one * 0.5f);
    }
}
