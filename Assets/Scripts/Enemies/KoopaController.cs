using UnityEngine;

/// <summary>
/// Koopa-like enemy with three states:
///  Walking  - patrols like a Goomba; stomp -> Shell.
///  Shell    - idle; side touch kicks it (slide away); stomp -> bounce only.
///  Sliding  - fast shell; kills other enemies, damages player on side; stomp -> stops.
/// </summary>
public class KoopaController : EnemyBase, IStompable
{
    private enum KState { Walking, Shell, Sliding }

    [SerializeField] private KState state = KState.Walking;
    [SerializeField] private float shellSpeed = 9f;
    [SerializeField] private float shellSquash = 0.6f;
    [SerializeField] private Sprite shellSprite;
    [SerializeField] private Sprite[] spinFrames;
    [SerializeField] private float spinFps = 14f;

    private int shellDir = 1;

    protected override void FixedUpdate()
    {
        if (dead || config == null) return;
        if (state == KState.Walking)
        {
            base.FixedUpdate();
        }
        else if (state == KState.Sliding)
        {
            if (ShellHitsWall()) shellDir = -shellDir;
            rb.linearVelocity = new Vector2(shellDir * shellSpeed, rb.linearVelocity.y);
        }
        else // Shell idle
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    private bool ShellHitsWall()
    {
        Bounds b = col.bounds;
        float dirX = Mathf.Sign(shellDir);
        Vector2 origin = new Vector2(dirX > 0 ? b.max.x : b.min.x, b.center.y);
        return Physics2D.Raycast(origin, new Vector2(dirX, 0f), 0.12f, groundMask);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (dead) return;
        if (collision.collider.CompareTag("Player"))
        {
            HandlePlayer(collision);
        }
        else if (state == KState.Sliding)
        {
            var other = collision.collider.GetComponent<EnemyBase>();
            if (other != null && other != this && !other.IsDead) other.Die();
        }
    }

    private void HandlePlayer(Collision2D collision)
    {
        var player = collision.collider.GetComponent<PlayerController>();
        if (player == null) player = collision.collider.GetComponentInParent<PlayerController>();
        var prb = collision.collider.attachedRigidbody;
        bool fromAbove = collision.collider.bounds.min.y > col.bounds.center.y;
        bool movingDown = prb == null || prb.linearVelocity.y <= 1f;

        if (fromAbove && movingDown) { OnStomped(player); return; }

        // Side contact
        if (state == KState.Shell)
        {
            KickAwayFrom(collision.collider.bounds.center.x);
        }
        else // Walking or Sliding
        {
            DamagePlayer(collision.collider);
        }
    }

    public void OnStomped(PlayerController player)
    {
        if (dead) return;
        if (state == KState.Walking || state == KState.Sliding)
        {
            state = KState.Shell;
            ShowShell();
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
        // Shell idle + stomp: just bounce the player.
        if (player != null) player.Bounce(config != null ? config.stompBounce : 12f);
    }

    private void KickAwayFrom(float sourceX)
    {
        float dir = Mathf.Sign(transform.position.x - sourceX);
        shellDir = dir == 0 ? 1 : (int)dir;
        state = KState.Sliding;
        var fb = GetComponent<SpriteFlipbook>();
        if (fb != null && spinFrames != null && spinFrames.Length > 0)
        {
            fb.SetFps(spinFps);
            fb.Play(spinFrames);
        }
    }

    private void ShowShell()
    {
        var fb = GetComponent<SpriteFlipbook>();
        if (fb != null) fb.Stop();
        if (sr == null) return;
        if (shellSprite != null) sr.sprite = shellSprite;
        else
        {
            var t = sr.transform;
            t.localScale = new Vector3(t.localScale.x, t.localScale.y * shellSquash, 1f);
        }
    }
}
