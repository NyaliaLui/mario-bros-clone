using UnityEngine;

/// <summary>Question block: first bump spawns its content, then becomes an inert "used" block.</summary>
public class QuestionBlock : BumpableBlock
{
    public enum Content { Coin, Mushroom }

    [SerializeField] private Content content = Content.Coin;
    [SerializeField] private GameObject coinPopPrefab;
    [SerializeField] private GameObject mushroomPrefab;
    [SerializeField] private Sprite usedSprite;
    [SerializeField] private Color usedColor = new Color(0.5f, 0.35f, 0.2f);

    private bool used;
    private SpriteRenderer sr;

    protected override void Awake()
    {
        base.Awake();
        sr = GetComponent<SpriteRenderer>();
    }

    public override void OnBumpFromBelow(PlayerController player)
    {
        if (used) return;
        used = true;
        base.OnBumpFromBelow(player);
        SpawnContent();
        if (sr != null)
        {
            if (usedSprite != null) sr.sprite = usedSprite;
            sr.color = usedColor;
        }
    }

    private void SpawnContent()
    {
        Vector3 spawn = restPos + Vector3.up;
        if (content == Content.Coin)
        {
            if (GameManager.Instance != null) GameManager.Instance.AddCoin(1);
            if (coinPopPrefab != null) Instantiate(coinPopPrefab, spawn, Quaternion.identity);
        }
        else
        {
            if (mushroomPrefab != null) Instantiate(mushroomPrefab, spawn, Quaternion.identity);
        }
    }
}
