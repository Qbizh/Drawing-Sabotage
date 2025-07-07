using UnityEngine;
using DG.Tweening;

public abstract class DynamicItemBehaviour : ItemBehaviour
{


    public override void Initialize(byte[] textureBytes, float cardScore, Vector3 origin)
    {
        base.Initialize(textureBytes, cardScore, origin);

        itemObj.transform.localScale = Vector2.one * score / 100;
    }

    

    public override Vector2 GetBoardPos() // random position
    {
        var board = DrawingBoard.GetMainBoard();
        var boardCollider = board.GetComponent<BoxCollider2D>();

        var boardMin = boardCollider.bounds.min;
        var boardMax = boardCollider.bounds.max;

        var collider = itemObj.GetComponent<BoxCollider2D>();

        var itemMin = collider.bounds.min;
        var itemMax = collider.bounds.max;


        Vector2 randomPos = new Vector2(Random.Range(boardMin.x - itemMin.x, boardMax.x - itemMax.x), 
            Random.Range(boardMin.y - itemMin.y, boardMax.y - itemMax.y));

        return randomPos;
    }
}
