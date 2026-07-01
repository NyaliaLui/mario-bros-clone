using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMovementConfig", menuName = "Mario/Player Movement Config")]
public class PlayerMovementConfig : ScriptableObject
{
    [Header("Speeds")]
    public float walkSpeed = 6f;
    public float runSpeed = 9f;

    [Header("Acceleration")]
    public float groundAccel = 60f;
    public float groundDecel = 70f;
    public float airAccel = 30f;
    public float turnDecel = 90f;

    [Header("Jump")]
    public float jumpVelocity = 14f;
    [Range(0f, 1f)] public float jumpCutMultiplier = 0.5f;
    public float maxFallSpeed = 18f;
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.12f;
    public float riseGravityScale = 3f;
    public float fallGravityScale = 5f;
}
