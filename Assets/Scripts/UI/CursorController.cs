using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CursorController : MonoBehaviour
{
    public static CursorController instance;

    Vector2 targetPos;

    Vector2 shakeOffset;
    Vector2 targetShake;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        //Cursor.visible = false;
        GetComponent<Image>().enabled = true;
    }

    private void Update()
    {
        transform.position = targetPos + shakeOffset;
    }

    public Vector2 UpdatePosition(Vector2 newPos)
    {
        targetPos = newPos;

        return transform.position;
    }

    public void StartShake(float strength, float speed, float duration)
    {
        DOVirtual.Float(0, 1, duration, t =>
        {
            if ((targetShake - shakeOffset).magnitude > 0.1f)
            {
                shakeOffset = Vector2.Lerp(shakeOffset, targetShake, speed * Time.deltaTime);
            }
            else
            {
                shakeOffset = new Vector2(Random.Range(-1f, 1f), (Random.Range(-1f, 1f)));
                shakeOffset.Normalize();

                shakeOffset *= strength * (1 - t * t);
            }
        });
    }
}
