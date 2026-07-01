using UnityEngine;

/// <summary>Power-up mushroom: on player contact, grows the player and awards score.</summary>
[RequireComponent(typeof(Collider2D))]
public class MushroomItem : MonoBehaviour
{
    [SerializeField] private int score = 1000;
    [SerializeField] private AudioClip pickupSfx;

    private bool collected;

    void OnCollisionEnter2D(Collision2D c) { TryCollect(c.collider); }
    void OnTriggerEnter2D(Collider2D c) { TryCollect(c); }

    private void TryCollect(Collider2D other)
    {
        if (collected || !other.CompareTag("Player")) return;
        var hp = other.GetComponent<PlayerHealth>();
        if (hp == null) hp = other.GetComponentInParent<PlayerHealth>();
        if (hp != null) hp.PowerUp();
        if (GameManager.Instance != null) GameManager.Instance.AddScore(score);
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(pickupSfx);
        collected = true;
        Destroy(gameObject);
    }
}
