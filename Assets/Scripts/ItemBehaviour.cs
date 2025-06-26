using UnityEngine;

public abstract class ItemBehaviour : MonoBehaviour
{
    public CardData cardData;

    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(byte[] textureBytes)
    {
        var sprite = spriteRenderer.sprite;
        
        var newTexture = new Texture2D(sprite.texture.width, sprite.texture.height, TextureFormat.RGBA32, false);
        newTexture.filterMode = FilterMode.Point;
        newTexture.wrapMode = TextureWrapMode.Clamp;

        newTexture.LoadImage(textureBytes);
        newTexture.Apply();

        sprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), Vector2.one * 0.5f);
        spriteRenderer.sprite = sprite;
    }

    public void Deploy()
    {
        Debug.Log("DEPLOYED " + cardData.cardName);
    }
}
