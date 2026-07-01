using UnityEngine;

/// <summary>
/// Velocity-driven dynamic Rigidbody2D platformer controller with classic
/// momentum, skid, variable jump height, coyote time and jump buffering.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerMovementConfig config;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private PlayerInputReader input;

    private Rigidbody2D rb;
    private CapsuleCollider2D col;
    private SpriteRenderer sr;

    private float coyoteCounter;
    private bool isJumping;

    public bool IsGrounded { get; private set; }
    
    public bool IsBig { get; set; }
    private Vector2 lastVelocity;
public Rigidbody2D Body => rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        if (input == null) input = GetComponent<PlayerInputReader>();
    }

    void FixedUpdate()
    {
        if (config == null || input == null) return;

        IsGrounded = CheckGrounded();
        float dt = Time.fixedDeltaTime;

        // ----- Horizontal movement -----
        float speed = input.SprintHeld ? config.runSpeed : config.walkSpeed;
        float target = input.MoveX * speed;
        float vx = rb.linearVelocity.x;
        float accel;
        if (Mathf.Abs(target) > 0.01f)
        {
            bool turning = (Mathf.Sign(target) != Mathf.Sign(vx)) && Mathf.Abs(vx) > 0.01f;
            if (turning) accel = IsGrounded ? config.turnDecel : config.airAccel;
            else accel = IsGrounded ? config.groundAccel : config.airAccel;
        }
        else
        {
            accel = IsGrounded ? config.groundDecel : config.airAccel * 0.5f;
        }
        vx = Mathf.MoveTowards(vx, target, accel * dt);

        // ----- Coyote time -----
        if (IsGrounded) coyoteCounter = config.coyoteTime;
        else coyoteCounter -= dt;

        float vy = rb.linearVelocity.y;

        // ----- Jump (buffered press + coyote window) -----
        bool buffered = (Time.time - input.LastJumpPressedTime) <= config.jumpBufferTime;
        if (buffered && coyoteCounter > 0f)
        {
            vy = config.jumpVelocity;
            isJumping = true;
            coyoteCounter = 0f;
            input.ConsumeJump();
        }

        // ----- Variable height: jump cut on release -----
        if (isJumping && !input.JumpHeld && vy > 0f)
        {
            vy *= config.jumpCutMultiplier;
            isJumping = false;
        }
        if (vy <= 0f) isJumping = false;

        // ----- Gravity split (floatier rise while held, snappier fall) -----
        rb.gravityScale = (vy > 0f && input.JumpHeld) ? config.riseGravityScale : config.fallGravityScale;

        // ----- Clamp fall speed -----
        if (vy < -config.maxFallSpeed) vy = -config.maxFallSpeed;

        rb.linearVelocity = new Vector2(vx, vy);
        lastVelocity = rb.linearVelocity;

        // ----- Facing -----
        if (Mathf.Abs(input.MoveX) > 0.01f && sr != null) sr.flipX = input.MoveX < 0f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Head-bump: only when moving upward into the underside of a bumpable block.
        if (lastVelocity.y <= 1f) return;
        var bump = collision.collider.GetComponent<IBumpable>();
        if (bump == null) return;
        float headY = col.bounds.max.y - 0.15f;
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).point.y >= headY)
            {
                bump.OnBumpFromBelow(this);
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -1f);
                break;
            }
        }
    }


    /// <summary>Bounce the player upward (used by enemy stomps).</summary>
    public void Bounce(float force)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, force);
        isJumping = false;
    }

    private bool CheckGrounded()
    {
        Bounds b = col.bounds;
        Vector2 center = new Vector2(b.center.x, b.min.y - 0.05f);
        Vector2 size = new Vector2(b.size.x * 0.9f, 0.12f);
        return Physics2D.OverlapBox(center, size, 0f, groundLayer) != null;
    }

    void OnDrawGizmosSelected()
    {
        var c = GetComponent<CapsuleCollider2D>();
        if (c == null) return;
        Bounds b = c.bounds;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(b.center.x, b.min.y - 0.05f, 0f), new Vector3(b.size.x * 0.9f, 0.12f, 0f));
    }
}
