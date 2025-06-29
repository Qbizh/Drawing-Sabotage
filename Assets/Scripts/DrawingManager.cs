using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class DrawingManager : MonoBehaviour
{
    public static DrawingManager instance;

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

    Vector2 mouseInput = Vector2.zero;

    Vector2Int point;
    Vector2Int lastPoint;

    void OnEnable()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }
        Debug.Log(InputManager.instance);

        InputManager.instance.onUseTool += OnUseTool;
        InputManager.instance.onMouseMove += OnMouseMove;
        InputManager.instance.onUndo += OnUndo;
        InputManager.instance.onRedo += OnRedo;

        currentTool = tools[0];
    }

    private void OnDisable()
    {
        InputManager.instance.onUseTool -= OnUseTool;
        InputManager.instance.onMouseMove -= OnMouseMove;
        InputManager.instance.onUndo -= OnUndo;
        InputManager.instance.onRedo -= OnRedo;
    }

    void Update()
    {
        UpdateTool();
    }

    private void UpdateTool()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(mouseInput);

        Collider2D[] results = new Collider2D[1];
        var filter = new ContactFilter2D();
        filter.useLayerMask = true;
        filter.layerMask = LayerMask.GetMask("Drawable");

        Physics2D.OverlapPoint(mousePos, filter, results);

        if (results[0] != null && results[0].TryGetComponent<DrawingBoard>(out currentBoard))
        {
            if (lastBoard != currentBoard)
            {
                lastPoint = -Vector2Int.one;
            }

            point = currentBoard.GetPointOnBoard(mousePos);

            currentTool.OnUpdate(point, lastPoint, InputManager.instance.UseToolDown());

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

    void OnUseTool()
    {
        if (currentBoard != null)
        {
            currentTool.OnUse(point, lastPoint);
            currentBoard.ApplyChanges();
        }
    }

    void OnMouseMove(Vector2 newPos)
    {
        mouseInput = newPos;
    }

    void OnUndo()
    {
        if (currentBoard != null)
        {
            currentBoard.UnDo();
        }
    }

    void OnRedo()
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
        if (currentBoard != null)
        {
            if (currentColor.a == 0)
            {
                return currentBoard.defaultBackground;
            }
        }

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