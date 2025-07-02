using UnityEngine;
using DG.Tweening;

public abstract class DynamicItemBehaviour : ItemBehaviour
{
    float deploySpeed = 20;

    public override void Deploy()
    {
        Vector2 startPos = transform.position;
        Vector2 endPos = GetRandomBoardPos();

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

    public abstract void Activate();


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

    private Vector2 GetRandomBoardPos()
    {
        var board = DrawingBoard.GetMainBoard();
        var boardCollider = board.GetComponent<BoxCollider2D>();

        var boardMin = boardCollider.bounds.min;
        var boardMax = boardCollider.bounds.max;

        var collider = GetComponent<BoxCollider2D>();

        var itemMin = collider.bounds.min;
        var itemMax = collider.bounds.max;


        Vector2 randomPos = new Vector2(Random.Range(boardMin.x - itemMin.x, boardMax.x - itemMax.x), 
            Random.Range(boardMin.y - itemMin.y, boardMax.y - itemMax.y));

        

        /*if (randomPos.x + itemMax.x > boardMax.x)
        {
            randomPos.x = boardMax.x - itemMax.x;
        } else if (randomPos.x + itemMin.x < boardMin.x)
        {
            randomPos.x = boardMin.x - itemMin.x;
        }

        if (randomPos.y + itemMax.y > boardMax.y)
        {
            randomPos.y = boardMax.y - itemMax.y;
        }
        else if (randomPos.y + itemMin.y < boardMin.y)
        {
            randomPos.y = boardMin.y - itemMin.y;
        }*/


        return randomPos;
    }
}
