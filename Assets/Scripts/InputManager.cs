using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputManager : MonoBehaviour, PlayerInputActions.IBoardActions, PlayerInputActions.IPromptGeneratorActions, PlayerInputActions.IItemPickingActions, PlayerInputActions.IAlwaysEnabledActions
{
    [SerializeField] CursorController cursorController;

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
        playerInput.PromptGenerator.AddCallbacks(this);
        playerInput.ItemPicking.AddCallbacks(this);
        playerInput.AlwaysEnabled.AddCallbacks(this);

        playerInput.AlwaysEnabled.Enable();

        PhaseHandler.phaseStart += OnPhaseStart;
    }

    private void SwitchActionMap(InputActionMap map)
    {

        foreach (var actionMap in playerInput.asset.actionMaps)
        {
            if (actionMap != map && actionMap != (InputActionMap)playerInput.AlwaysEnabled)
            {
                actionMap.Disable();
            } else if (actionMap == map) 
            {
                actionMap.Enable();
            }
        }
    }

    private void OnPhaseStart(bool asServer)
    {
        if (asServer) return;

        var gamePhase = GamePhaseManager.instance.gamePhase.Value;

        switch (gamePhase) 
        {
            case GamePhaseManager.GamePhase.PromptInput:
                
                break;
            case GamePhaseManager.GamePhase.PromptGeneration:
                SwitchActionMap(playerInput.PromptGenerator);

                break;
            case GamePhaseManager.GamePhase.ItemPicking:
                SwitchActionMap(playerInput.ItemPicking);

                break;
            case GamePhaseManager.GamePhase.Drawing:
                SwitchActionMap(playerInput.Board);

                break;
            case GamePhaseManager.GamePhase.Voting:

                break;
        }
    }

    // Always Enabled
    public event Action<Vector2> onMouseMove;
    public void OnMousePosition(InputAction.CallbackContext ctx)
    {
        Vector2 pos = cursorController.UpdatePosition(ctx.ReadValue<Vector2>());

        onMouseMove?.Invoke(pos);
    }


    // Prompt Generating Phase
    public event Action onReRoll;

    public void OnReRoll(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            onReRoll?.Invoke();
        }
    }

    // Item Picking Phase

    public event Action onPickup;
    public event Action onJump;

    public void OnPickup(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            onPickup?.Invoke();
        }
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            onJump?.Invoke();
        }
    }


    // Drawing Phase

    public event Action onUseTool;
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
