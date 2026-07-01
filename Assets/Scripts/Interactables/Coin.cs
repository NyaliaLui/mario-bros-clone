using UnityEngine;

/// <summary>Collectible coin. Trigger-based; credits coins + score on pickup.</summary>
[RequireComponent(typeof(Collider2D))]
public class Coin : MonoBehaviour
{
    [SerializeField] private int value = 1;
    [SerializeField] private AudioClip pickupSfx;

    void Reset()
    {
        var c = GetComponent<Collider2D>();
        if (c != null) c.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (GameManager.Instance != null) GameManager.Instance.AddCoin(value);
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(pickupSfx);
        Destroy(gameObject);
    }
}
