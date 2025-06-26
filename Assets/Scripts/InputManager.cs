using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputManager : MonoBehaviour, PlayerInputActions.IBoardActions
{
    public static InputManager instance { get; private set; }
    PlayerInputActions playerInput;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this);
        }

        playerInput = new PlayerInputActions();
        playerInput.Board.AddCallbacks(this);

        playerInput.Board.Enable();
    }


    public event Action onUseTool;
    public event Action <Vector2> onMouseMove;
    public event Action onUndo;
    public event Action onRedo;
    public event Action onGrab;

    public void OnUseTool(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            onUseTool?.Invoke();
        }
    }

    public void OnMousePosition(InputAction.CallbackContext ctx)
    {
        onMouseMove?.Invoke(ctx.ReadValue<Vector2>());
    }

    public void OnUndo(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            onUndo?.Invoke();
        }
    }

    public void OnRedo(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            onRedo?.Invoke();
        }
    }

    public void OnGrab(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            onGrab?.Invoke();
        }
    }



    public bool UseToolDown()
    {
        return playerInput.Board.UseTool.IsPressed();
    }

    public bool GrabDown()
    {
        return playerInput.Board.Grab.IsPressed();
    }
}
