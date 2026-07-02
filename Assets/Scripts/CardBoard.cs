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

    private new void Awake()
    {
        base.Awake();

        completionDisplay = GetComponentInChildren<TMP_Text>();
        deckAnimator = GetComponentInChildren<Animator>();
    }

    private new void OnEnable()
    {
        base.OnEnable();
        textureChanged += UpdateScore;

        ItemsManager.onItemDraw += DrawCard;
    }

    private new void OnDisable()
    {
        base.OnDisable();

        cardScore = 0;
        DisplayScore();

        textureChanged -= UpdateScore;

        ItemsManager.onItemDraw -= DrawCard;
    }

    private void DrawCard()
    {
        deckAnimator.SetTrigger("Draw");
    }

    public void LoadCardData()
    {
        var oldSprite = cardDisplay.sprite;

        cardDisplay.sprite = ItemsManager.instance.GetCurrentItem().GenerateSprite(oldSprite.texture.width, oldSprite.texture.height);

        GenerateForegroundMask();
    }

    void GenerateForegroundMask()
    {
        foregroundLength = 0;

        var pixels = ItemsManager.instance.GetCurrentItem().texture.GetPixels();
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
        if (ItemsManager.instance.GetCurrentItem() == null) return;

        var boardPixels = texture.GetPixels();
        var cardPixels = ItemsManager.instance.GetCurrentItem().texture.GetPixels();

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

        DisplayScore();
    }

    private void DisplayScore()
    {
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
