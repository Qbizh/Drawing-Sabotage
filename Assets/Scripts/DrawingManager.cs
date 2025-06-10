using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class DrawingManager : MonoBehaviour
{
    public static DrawingManager instance;

    PlayerInputActions inputActions;

    InputAction useToolInput;
    InputAction mousePosInput;

    [SerializeField] Tool[] tools =
    {
        new Draw(),
        new Erase(),
        new Fill(),
        new EyeDropper(),
        new Line()
    };

    private Tool currentTool = null;

    [SerializeField]private DrawingBoard currentBoard;
    private DrawingBoard lastBoard;

    [SerializeField] private Color currentColor = Color.black;

    [SerializeField] private float strokeSize = 1.0f;

    public int UndoHistoryLength = 5;

    Vector2Int point;
    Vector2Int lastPoint;

    private void OnEnable()
    {
        inputActions = new PlayerInputActions();

        useToolInput = inputActions.Board.UseTool;
        mousePosInput = inputActions.Board.MousePosition;

        useToolInput.performed += OnUseTool;
        inputActions.Board.Undo.performed += OnUndo;
        inputActions.Board.Redo.performed += OnRedo;

        inputActions.Board.Enable();
    }

    private void OnDisable()
    {
        inputActions.Board.Disable();
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }

        currentTool = tools[0];
    }

    void Update()
    {
        UpdateTool();
    }

    private void UpdateTool()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(mousePosInput.ReadValue<Vector2>());

        Collider2D[] results = new Collider2D[1];
        var filter = new ContactFilter2D().NoFilter();

        Physics2D.OverlapPoint(mousePos, filter, results);

        if (results[0] != null && results[0].TryGetComponent<DrawingBoard>(out currentBoard))
        {
            if (lastBoard != currentBoard)
            {
                lastPoint = -Vector2Int.one;
            }

            point = currentBoard.GetPointOnBoard(mousePos);

            currentTool.OnUpdate(point, lastPoint, useToolInput.IsPressed());

            currentBoard.ApplyChanges();

            lastPoint = point;
        } else
        {
            currentBoard = null;
        }
        
        if (lastBoard != currentBoard)
        {
            currentTool.OnBoardChanged();
        }

        lastBoard = currentBoard;
    }

    void OnUseTool(InputAction.CallbackContext ctx)
    {
        if (currentBoard != null)
        {
            currentTool.OnUse(point, lastPoint);
            currentBoard.ApplyChanges();
        }
    }

    void OnUndo(InputAction.CallbackContext ctx)
    {
        if (currentBoard != null)
        {
            currentBoard.UnDo();
        }
    }

    void OnRedo(InputAction.CallbackContext ctx)
    {
        if (currentBoard != null)
        {
            currentBoard.ReDo();
        }
    }


    public DrawingBoard GetBoard()
    {
        return currentBoard;
    }

    public Color GetColor()
    {
        return currentColor;
    }

    public void SetColor(Color newColor)
    {
        currentColor = newColor;
    }

    public void SetTool(int toolIndex)
    {
        currentTool = tools[toolIndex];
    }

    public int GetToolIndex()
    {
        Debug.Log(currentTool.GetType());
        return Array.FindIndex<Tool>(tools, t => t.GetType() == currentTool.GetType());
    }

    public float GetStrokeSize()
    {
        return strokeSize;
    }

    public void SetStrokeSize(float newStroke)
    {
        strokeSize = newStroke;
    }
}