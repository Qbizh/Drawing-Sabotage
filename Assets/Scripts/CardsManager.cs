using UnityEngine;
using System;
using FishNet.Connection;
using System.Collections.Generic;

public class CardsManager : MonoBehaviour
{
    public static CardsManager instance;

    [SerializeField] GameObject cardPrefab;

    [SerializeField] CardDatabase cardDatabase;

    [SerializeField] bool qualityOverride = true;

    GameObject cardObj;

    [SerializeField] CardData currentCard;
    float currentCardScore = 0;

    [SerializeField] List<CardData> playerDeck;
    [SerializeField] Queue<CardData> deck = new Queue<CardData>();

    public static event Action onCardDraw;

    Vector3 mousePos;

    bool holdingCard = false;

    void Start()
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
            Init(cardDatabase.GetDeck(PlayerDataHolder.instance.playerData.deck));
        }
    }

    public void Init(List<CardData> newDeck)
    {
        playerDeck = newDeck;

        InputManager.instance.onGrab += OnGrab;
        InputManager.instance.onMouseMove += OnMouseMove;

        cardObj = Instantiate(cardPrefab);
        cardObj.SetActive(false);

        ShuffleDeck(playerDeck);
        DrawCard();
    }

    private void OnDisable()
    {
        InputManager.instance.onGrab -= OnGrab;
        InputManager.instance.onMouseMove -= OnMouseMove;
        PhaseHandler.phaseStart -= OnGameStart;
    }

    public void ShuffleDeck(List<CardData> d)
    {
        deck.Clear();
        
        d = new List<CardData>(d);

        while (d.Count > 0)
        {
            var card = d[UnityEngine.Random.Range(0, d.Count - 1)];
            deck.Enqueue(card);

            d.Remove(card);
        }
    }

    void OnGrab()
    {
        CardBoard cardBoard;

        var currentBoard = DrawingManager.instance.GetBoard();

        if (currentCard != null && !holdingCard && currentBoard != null && currentBoard.TryGetComponent<CardBoard>(out cardBoard) && (cardBoard.GetScore() > 50f || qualityOverride))
        {
            holdingCard = true;
            currentCardScore = cardBoard.GetScore();

            var drawingPixels = cardBoard.GrabCard();
            var texture = new Texture2D(currentCard.texture.width, currentCard.texture.height);

            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;

            texture.SetPixels(drawingPixels);
            texture.Apply();

            var newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
            cardObj.GetComponent<SpriteRenderer>().sprite = newSprite;

            cardObj.SetActive(true);

            cardObj.transform.position = new Vector3(mousePos.x, mousePos.y, -1);
        }
    }

    void OnMouseMove(Vector2 newPos)
    {
        mousePos = Camera.main.ScreenToWorldPoint(newPos);

        if (holdingCard)
        {
            cardObj.transform.position = new Vector3(mousePos.x, mousePos.y, -1);
        }
    }

    public CardData GetCurrentCard()
    {
        return currentCard;
    }

    public bool IsHoldingCard()
    {
        return holdingCard;
    }

    public bool SendCard(NetworkConnection targetClient)
    {
        if (!holdingCard) return false;

        PipesManager.instance.SendItemToClient(targetClient, cardDatabase.GetCardId(currentCard), currentCardScore, cardObj.GetComponent<SpriteRenderer>().sprite.texture.EncodeToPNG());

        cardObj = Instantiate(cardPrefab);
        cardObj.SetActive(false);

        holdingCard = false;

        DrawCard();

        return true;
    }

    private void DrawCard()
    {
        if (deck.Count > 0)
        {
            currentCard = deck.Dequeue();

            onCardDraw?.Invoke();
        } else
        {
            currentCard = null;
        }
    }

}