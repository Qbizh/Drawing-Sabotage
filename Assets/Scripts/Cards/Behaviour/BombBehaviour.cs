using UnityEngine;

public class BombBehaviour : DynamicItemBehaviour
{
    Animator animator;

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
       

        board.StampTexture(craterStamp, score / 100, Random.Range(0, 360), pos);
        board.ApplyChanges();

        board.ClearHistory();

        Destroy(gameObject);
    }
}
