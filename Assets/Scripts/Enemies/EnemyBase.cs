using UnityEngine;

/// <summary>Shared walker enemy: patrols, turns at walls (and ledges if configured), can die.</summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public abstract class EnemyBase : MonoBehaviour
{
    [SerializeField] protected EnemyConfig config;
    [SerializeField] protected LayerMask groundMask;
    [SerializeField] protected int facing = -1;

    protected Rigidbody2D rb;
    protected Collider2D col;
    protected SpriteRenderer sr;
    protected bool dead;

    public bool IsDead => dead;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    protected virtual void FixedUpdate()
    {
        if (dead || config == null) return;
        if (ShouldTurn()) facing = -facing;
        rb.linearVelocity = new Vector2(facing * config.moveSpeed, rb.linearVelocity.y);
    }

    protected virtual bool ShouldTurn()
    {
        Bounds b = col.bounds;
        float dirX = Mathf.Sign(facing);
        float frontX = dirX > 0 ? b.max.x : b.min.x;
        Vector2 wallOrigin = new Vector2(frontX, b.center.y);
        bool wall = Physics2D.Raycast(wallOrigin, new Vector2(dirX, 0f), 0.12f, groundMask);
        if (wall) return true;
        if (config.turnsAtLedge)
        {
            Vector2 ledgeOrigin = new Vector2(frontX + dirX * 0.1f, b.min.y + 0.02f);
            bool ground = Physics2D.Raycast(ledgeOrigin, Vector2.down, 0.4f, groundMask);
            if (!ground) return true;
        }
        return false;
    }

    public virtual void Die()
    {
        if (dead) return;
        dead = true;
        if (GameManager.Instance != null && config != null) GameManager.Instance.AddScore(config.scoreValue);
        if (col != null) col.enabled = false;
        if (rb != null) rb.simulated = false;
        Destroy(gameObject, 0.3f);
    }

    protected void DamagePlayer(Collider2D playerCol)
    {
        var hp = playerCol.GetComponent<PlayerHealth>();
        if (hp == null) hp = playerCol.GetComponentInParent<PlayerHealth>();
        if (hp != null) hp.TakeDamage(1, transform.position);
    }
}
