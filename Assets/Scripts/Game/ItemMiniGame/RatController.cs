using FishNet.Object;
using UnityEngine;

public class RatController : NetworkBehaviour
{
    Rigidbody2D rb;

    [SerializeField] float moveSpeed = 3f;

    Vector2 mousePosition = Vector2.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        InputManager.instance.onMouseMove += OnMouseMove;
    }

    private void OnDisable()
    {
        InputManager.instance.onMouseMove -= OnMouseMove;
    }

    private void Update()
    {
        if (!IsOwner) return;

        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector2 dir = (mouseWorldPos - (Vector2)transform.position).normalized;

        transform.up = dir;

        rb.linearVelocity = dir * moveSpeed;
    }

    private void OnMouseMove(Vector2 newPos)
    {
        mousePosition = newPos;
    }
}
