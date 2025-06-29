using UnityEngine;

public class BombBehaviour : DynamicItemBehaviour
{
    Animator animator;

    float maxRadius = 200;

    [SerializeField] Texture2D craterStamp;

    private void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
    }

    public override void Activate()
    {
        animator.SetTrigger("Activate");
    }

    private void OnExplode()
    {
        var board = DrawingBoard.GetMainBoard();

        var pos = board.GetPointOnBoard(transform.position);
        
        board.StampTexture(craterStamp, 1, pos);
        board.ApplyChanges();

        Destroy(gameObject);
    }
}
