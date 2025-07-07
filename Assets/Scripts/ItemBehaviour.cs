using DG.Tweening;
using UnityEngine;

public abstract class ItemBehaviour : MonoBehaviour
{
    public CardData cardData;

    public GameObject itemObj;

    SpriteRenderer spriteRenderer;

    public float score = 0;

    float deploySpeed = 20;

    public void Awake()
    {
        spriteRenderer = itemObj.GetComponent<SpriteRenderer>();
    }

    public virtual void Initialize(byte[] textureBytes, float cardScore, Vector3 origin)
    {
        score = cardScore;

        var sprite = spriteRenderer.sprite;
        
        var newTexture = new Texture2D(sprite.texture.width, sprite.texture.height, TextureFormat.RGBA32, false);
        newTexture.filterMode = FilterMode.Point;
        newTexture.wrapMode = TextureWrapMode.Clamp;

        newTexture.LoadImage(textureBytes);
        newTexture.Apply();

        sprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), Vector2.one * 0.5f);
        spriteRenderer.sprite = sprite;

        transform.position = origin;
    }

    public void Deploy()
    {
        Vector2 startPos = transform.position;
        Vector2 endPos = GetBoardPos();

        Vector2 controlPoint = new Vector2(endPos.x, startPos.y);


        Vector2 last = startPos;

        float dist = ApproximateBezierArcLength(20, startPos, controlPoint, endPos);
        float duration = dist / deploySpeed;


        DOVirtual.Float(0, 1, duration, t =>
        {
            transform.position = SampleBezier(t, startPos, controlPoint, endPos);

            transform.Rotate(transform.forward * deploySpeed * Time.deltaTime * 15);

        }).SetEase(Ease.Linear).OnComplete(() =>
        {
            transform.rotation = Quaternion.identity;

            Activate();
        });
    }


    private Vector2 SampleBezier(float t, Vector2 startPos, Vector2 controlPoint, Vector2 endPos)
    {
        return Mathf.Pow(1 - t, 2) * startPos + 2 * (1 - t) * t * controlPoint + t * t * endPos;
    }

    private float ApproximateBezierArcLength(int resolution, Vector2 startPos, Vector2 controlPoint, Vector2 endPos)
    {
        float length = 0;
        Vector2 lastPos = startPos;

        for (int i = 1; i < resolution; i++)
        {
            Vector2 pos = SampleBezier((float)i / resolution, startPos, controlPoint, endPos);

            length += Vector2.Distance(lastPos, pos);
            lastPos = pos;
        }

        return length;
    }

    public abstract void Activate();

    public abstract Vector2 GetBoardPos(); 
}
