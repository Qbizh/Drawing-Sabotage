using UnityEngine;
using System.Linq;
using System;
using TMPro;

public class CardBoard : DrawingBoard
{
    [SerializeField] GameObject cardPrefab;

    Animator deckAnimator;

    [SerializeField] SpriteRenderer cardDisplay;
    TMP_Text completionDisplay;

    bool[] foregroundMask = null;
    int foregroundLength = 0;

    [SerializeField] float backgroundWeight = 3.5f;

    float cardScore = 0;


    public void Start()
    {
        base.Start();

        completionDisplay = GetComponentInChildren<TMP_Text>();
        deckAnimator = GetComponentInChildren<Animator>();

        textureChanged += UpdateScore;
        
        CardsManager.instance.onCardDraw += DrawCard;
        LoadCardData();
    }

    private void DrawCard()
    {
        deckAnimator.SetTrigger("Draw");
    }

    public void LoadCardData()
    {
        //base.enabled = true;

        var oldSprite = cardDisplay.sprite;

        var newTexture = new Texture2D(oldSprite.texture.width, oldSprite.texture.height, TextureFormat.RGBA32, false);
        newTexture.filterMode = FilterMode.Point;
        newTexture.wrapMode = TextureWrapMode.Clamp;

        newTexture.CopyPixels(CardsManager.instance.GetCurrentCard().texture);
        newTexture.Apply();

        var newSprite = Sprite.Create(newTexture, new Rect(0, 0, oldSprite.texture.width, oldSprite.texture.height), Vector2.one * 0.5f);
        cardDisplay.sprite = newSprite;

        GenerateForegroundMask();
    }

    void GenerateForegroundMask()
    {
        foregroundLength = 0;

        var pixels = CardsManager.instance.GetCurrentCard().texture.GetPixels();
        foregroundMask = pixels.Select(c => {
            if (c.a > 0)
            {
                foregroundLength++;
            }

            return c.a > 0;
        }).ToArray();
    }

    private void UpdateScore()
    {
        if (CardsManager.instance.GetCurrentCard() == null) return;

        var boardPixels = texture.GetPixels();
        var cardPixels = CardsManager.instance.GetCurrentCard().texture.GetPixels();

        int correctForeground = 0;
        int incorrectBackground = 0;

        for (int i = 0; i < boardPixels.Length; i++)
        {

            if (TextureUtil.IsSameColor(boardPixels[i], cardPixels[i]))
            {
                if (foregroundMask[i])
                {
                    correctForeground++;
                }
            } else
            {
                if (!foregroundMask[i])
                {
                    incorrectBackground++;
                }
            }
        }

        float score = (float)correctForeground / foregroundLength - incorrectBackground * backgroundWeight / (foregroundMask.Length - foregroundLength);
        score = Mathf.Clamp01(score) * 10000;
        score = Mathf.Floor(score) / 100;

        cardScore = score;

        completionDisplay.text = (cardScore).ToString() + "%";
    }

    public Color[] GrabCard()
    {
        var oldColors = texture.GetPixels();

        Color[] colors = new Color[texture.width * texture.height];
        Array.Fill(colors, defaultBackground);

        texture.SetPixels(colors);
        ApplyChanges();

        ClearHistory();

        cardDisplay.sprite.texture.SetPixels(colors);
        cardDisplay.sprite.texture.Apply();

        base.enabled = false;
        return oldColors;
    }

    public float GetScore()
    {
        return cardScore;
    }
}
