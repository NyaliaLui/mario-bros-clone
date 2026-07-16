using System.Collections;
using UnityEngine;

/// <summary>
/// Player power state (Small/Big), power-ups, damage with i-frames, and death.
/// </summary>
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerHealth : MonoBehaviour, IDamageReceiver
{
    public enum PowerState { Small, Big }

    [SerializeField] private PowerState state = PowerState.Small;
    [SerializeField] private Transform visual;
    [SerializeField] private float invincibleTime = 1.5f;
    [SerializeField] private Vector2 smallSize = new Vector2(0.8f, 1.0f);
    [SerializeField] private Vector2 smallOffset = new Vector2(0f, 0f);
    [SerializeField] private Vector2 bigSize = new Vector2(0.8f, 1.8f);
    [SerializeField] private Vector2 bigOffset = new Vector2(0f, 0.4f);
    [SerializeField] private AudioClip powerUpSfx;
    [SerializeField] private AudioClip damageSfx;
    [SerializeField] private AudioClip deathSfx;

    private PlayerController controller;
    private CapsuleCollider2D col;
    private SpriteRenderer sr;
    private bool invincible;
    private bool dead;

    public bool IsBig => state == PowerState.Big;
    public bool IsDead => dead;
    public bool IsInvincible => invincible;

    void Awake()
    {
        controller = GetComponent<PlayerController>();
        col = GetComponent<CapsuleCollider2D>();
        if (visual != null) sr = visual.GetComponent<SpriteRenderer>();
        ApplyState();
    }

    private void ApplyState()
    {
        bool big = state == PowerState.Big;
        controller.IsBig = big;
        Vector2 size = big ? bigSize : smallSize;
        Vector2 off = big ? bigOffset : smallOffset;
        if (col != null) { col.size = size; col.offset = off; }
        if (visual != null)
        {
            // Real Small/Big art has correct proportions — no scaling, just recenter.
            visual.localScale = Vector3.one;
            visual.localPosition = new Vector3(off.x, off.y, 0f);
            var anim = visual.GetComponent<PlayerSpriteAnimator>();
            if (anim != null) anim.SetBig(big);
        }
    }

    public void PowerUp()
    {
        if (state == PowerState.Big)
        {
            if (GameManager.Instance != null) GameManager.Instance.AddScore(1000);
            return;
        }
        state = PowerState.Big;
        ApplyState();
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(powerUpSfx);
    }

    public void TakeDamage(int amount, Vector2 source)
    {
        if (invincible || dead) return;
        if (state == PowerState.Big)
        {
            state = PowerState.Small;
            ApplyState();
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(damageSfx);
            StartCoroutine(InvincibilityFlash(invincibleTime));
        }
        else
        {
            Die(source);
        }
    }

    public void Die(Vector2 source)
    {
        if (dead) return;
        dead = true;
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(deathSfx);
        controller.enabled = false;
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(0f, 8f);
            rb.gravityScale = 3f;
        }
        if (GameManager.Instance != null) GameManager.Instance.RegisterPlayerDeath();
    }

    private IEnumerator InvincibilityFlash(float duration)
    {
        invincible = true;
        float t = 0f;
        while (t < duration)
        {
            t += 0.1f;
            if (sr != null) sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(0.1f);
        }
        if (sr != null) sr.enabled = true;
        invincible = false;
    }
}
