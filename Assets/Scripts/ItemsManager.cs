using UnityEngine;
using System;
using FishNet.Connection;
using System.Collections.Generic;

public class ItemsManager : MonoBehaviour
{
    public static ItemsManager instance;

    [SerializeField] CardBoard itemBoard;

    [SerializeField] GameObject itemPrefab;

    [SerializeField] CardDatabase itemDatabase;

    [SerializeField] bool qualityOverride = true;

    GameObject itemObj;

    [SerializeField] CardData currentItem;
    float currentItemScore = 0;

    [SerializeField] List<CardData> playerDeck;
    [SerializeField] Queue<CardData> items = new Queue<CardData>();

    public static event Action onItemDraw;

    Vector3 mousePos;

    bool holdingItem = false;

    void Start()//
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this);
        }
    }

    private void OnEnable()
    {
        PhaseHandler.phaseStart += OnGameStart;
    }

    private void OnGameStart(bool asServer)
    {
        if (!asServer)
        {
            Init();
        }
    }

    public void Init()
    {
        InputManager.instance.onGrab += OnGrab;
        InputManager.instance.onMouseMove += OnMouseMove;

        itemObj = Instantiate(itemPrefab);
        itemObj.SetActive(false);

        ShuffleDeck(playerDeck);
        DrawItem();
    }

    private void OnDisable()
    {
        InputManager.instance.onGrab -= OnGrab;
        InputManager.instance.onMouseMove -= OnMouseMove;
        PhaseHandler.phaseStart -= OnGameStart;
    }

    public void ShuffleDeck(List<CardData> d)
    {
        items.Clear();
        
        d = new List<CardData>(d);

        while (d.Count > 0)
        {
            var item = d[UnityEngine.Random.Range(0, d.Count - 1)];
            items.Enqueue(item);

            d.Remove(item);
        }
    }

    void OnGrab()
    {
        var currentBoard = DrawingManager.instance.GetBoard();

        if (currentItem != null && !holdingItem && currentBoard != null && currentBoard == itemBoard && (itemBoard.GetScore() > 50f || qualityOverride))
        {
            holdingItem = true;
            currentItemScore = itemBoard.GetScore();

            var drawingPixels = itemBoard.GrabCard();
            var texture = new Texture2D(currentItem.texture.width, currentItem.texture.height);

            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;

            texture.SetPixels(drawingPixels);
            texture.Apply();

            var newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
            itemObj.GetComponent<SpriteRenderer>().sprite = newSprite;

            itemObj.SetActive(true);

            itemObj.transform.position = new Vector3(mousePos.x, mousePos.y, -1);
        }
    }

    void OnMouseMove(Vector2 newPos)
    {
        mousePos = Camera.main.ScreenToWorldPoint(newPos);

        if (holdingItem)
        {
            itemObj.transform.position = new Vector3(mousePos.x, mousePos.y, -1);
        }
    }

    public CardData GetCurrentItem()
    {
        return currentItem;
    }

    public bool IsHoldingitem()
    {
        return holdingItem;
    }

    public bool SendItem(NetworkConnection targetClient)
    {
        if (!holdingItem) return false;

        PipesManager.instance.SendItemToClient(targetClient, itemDatabase.GetCardId(currentItem), currentItemScore, itemObj.GetComponent<SpriteRenderer>().sprite.texture.EncodeToPNG());

        itemObj = Instantiate(itemPrefab);
        itemObj.SetActive(false);

        holdingItem = false;

        DrawItem();

        return true;
    }

    private void DrawItem()
    {
        if (items.Count > 0)
        {
            currentItem = items.Dequeue();

            itemBoard.enabled = true;

            onItemDraw?.Invoke();
        } else
        {
            currentItem = null;
        }
    }

}