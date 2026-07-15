using UnityEngine;

/// <summary>
/// Drives the player's sprite from movement state: idle / walk (speed-scaled
/// fps) / jump / skid, with separate frame sets for Small and Big forms.
/// Lives on the Visual child next to the SpriteRenderer.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerSpriteAnimator : MonoBehaviour
{
    [Header("Small form")]
    [SerializeField] private Sprite smallIdle;
    [SerializeField] private Sprite[] smallWalk;
    [SerializeField] private Sprite smallJump;
    [SerializeField] private Sprite smallSkid;

    [Header("Big form")]
    [SerializeField] private Sprite bigIdle;
    [SerializeField] private Sprite[] bigWalk;
    [SerializeField] private Sprite bigJump;
    [SerializeField] private Sprite bigSkid;

    [Header("Tuning")]
    [SerializeField] private float walkFpsAtRun = 12f;
    [SerializeField] private float minWalkFps = 5f;
    [SerializeField] private float runSpeedReference = 9f;

    private SpriteRenderer sr;
    private PlayerController controller;
    private PlayerInputReader input;
    private bool isBig;
    private float walkClock;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        controller = GetComponentInParent<PlayerController>();
        input = GetComponentInParent<PlayerInputReader>();
    }

    public void SetBig(bool big) { isBig = big; }

    void Update()
    {
        if (controller == null || sr == null) return;
        var rb = controller.Body;
        float vx = rb != null ? rb.linearVelocity.x : 0f;
        float ax = Mathf.Abs(vx);
        float moveX = input != null ? input.MoveX : 0f;

        Sprite idle = isBig ? bigIdle : smallIdle;
        Sprite jump = isBig ? bigJump : smallJump;
        Sprite skid = isBig ? bigSkid : smallSkid;
        Sprite[] walk = isBig ? bigWalk : smallWalk;

        // Facing: input wins, else velocity.
        if (Mathf.Abs(moveX) > 0.01f) sr.flipX = moveX < 0f;
        else if (ax > 0.05f) sr.flipX = vx < 0f;

        // Airborne
        if (!controller.IsGrounded)
        {
            sr.sprite = jump != null ? jump : idle;
            walkClock = 0f;
            return;
        }

        // Skid: pushing against current velocity
        bool skidding = Mathf.Abs(moveX) > 0.01f && ax > 1f && Mathf.Sign(moveX) != Mathf.Sign(vx);
        if (skidding && skid != null)
        {
            sr.sprite = skid;
            walkClock = 0f;
            return;
        }

        // Walk / idle
        if (ax > 0.15f && walk != null && walk.Length > 0)
        {
            float fps = Mathf.Max(minWalkFps, walkFpsAtRun * Mathf.Clamp01(ax / runSpeedReference));
            walkClock += Time.deltaTime * fps;
            int index = (int)walkClock % walk.Length;
            sr.sprite = walk[index];
        }
        else if (idle != null)
        {
            sr.sprite = idle;
            walkClock = 0f;
        }
    }
}
