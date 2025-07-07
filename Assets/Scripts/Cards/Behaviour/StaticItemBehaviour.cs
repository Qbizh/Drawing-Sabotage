using UnityEngine;

public abstract class StaticItemBehaviour : ItemBehaviour
{
    public override Vector2 GetBoardPos()
    {
        return DrawingBoard.GetMainBoard().transform.position;
    }

    public override void Activate()
    {
        GetComponent<Animator>().SetTrigger("Activate");
    }

    public abstract void OnActivation();
}
