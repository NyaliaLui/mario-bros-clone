using System.Collections;
using UnityEngine;

/// <summary>Flagpole / end-of-level goal: completes the level and slides the player down.</summary>
[RequireComponent(typeof(Collider2D))]
public class LevelGoal : MonoBehaviour
{
    [SerializeField] private AudioClip clearSfx;
    [SerializeField] private float slideSpeed = 4f;
    [SerializeField] private float baseY = 0.5f;

    private bool triggered;

    void Reset()
    {
        var c = GetComponent<Collider2D>();
        if (c != null) c.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered || !other.CompareTag("Player")) return;
        triggered = true;

        var ctrl = other.GetComponent<PlayerController>();
        if (ctrl == null) ctrl = other.GetComponentInParent<PlayerController>();
        if (ctrl != null) ctrl.enabled = false;

        var rb = other.attachedRigidbody;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
            StartCoroutine(SlideDown(rb));
        }

        if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(clearSfx);
        if (GameManager.Instance != null) GameManager.Instance.CompleteLevel();
    }

    private IEnumerator SlideDown(Rigidbody2D rb)
    {
        // Snap to the pole and slide to the base.
        float x = transform.position.x;
        while (rb.position.y > baseY)
        {
            rb.position = new Vector2(x, Mathf.Max(baseY, rb.position.y - slideSpeed * Time.deltaTime));
            yield return null;
        }
        rb.position = new Vector2(x, baseY);
    }
}
