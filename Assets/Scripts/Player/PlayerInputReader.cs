using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Wraps the shared InputActionAsset and exposes plain fields so gameplay
/// scripts never touch Input System types directly. Also merges "virtual"
/// input from on-screen touch buttons (see TouchButton).
/// </summary>
public class PlayerInputReader : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputAsset;
    [SerializeField] private string actionMapName = "Player";

    private InputActionMap map;
    private InputAction moveAction, jumpAction, sprintAction, crouchAction, interactAction;

    // Virtual input from on-screen touch buttons.
    private bool vLeft, vRight, vJump, vJumpPrev;

    public float MoveX { get; private set; }
    public float MoveY { get; private set; }
    public bool JumpHeld { get; private set; }
    public bool SprintHeld { get; private set; }
    public bool CrouchHeld { get; private set; }
    public bool InteractHeld { get; private set; }

    /// <summary>Time.time of the most recent Jump press (for jump buffering).</summary>
    public float LastJumpPressedTime { get; private set; } = -999f;

    public void ConsumeJump() { LastJumpPressedTime = -999f; }

    // --- Virtual (touch) input API ---
    public void SetTouchLeft(bool pressed) { vLeft = pressed; }
    public void SetTouchRight(bool pressed) { vRight = pressed; }
    public void SetTouchJump(bool pressed) { vJump = pressed; }

    void Awake()
    {
        if (inputAsset == null)
        {
            Debug.LogError("PlayerInputReader: inputAsset not assigned.", this);
            enabled = false;
            return;
        }
        map = inputAsset.FindActionMap(actionMapName, true);
        moveAction = map.FindAction("Move", true);
        jumpAction = map.FindAction("Jump", true);
        sprintAction = map.FindAction("Sprint", true);
        crouchAction = map.FindAction("Crouch", true);
        interactAction = map.FindAction("Interact", true);
        jumpAction.performed += OnJumpPerformed;
    }

    void OnJumpPerformed(InputAction.CallbackContext ctx) { LastJumpPressedTime = Time.time; }

    void OnEnable() { if (map != null) map.Enable(); }
    void OnDisable() { if (map != null) map.Disable(); }

    void Update()
    {
        if (map == null) return;

        Vector2 mv = moveAction.ReadValue<Vector2>();
        float mvx = mv.x;
        int v = (vRight ? 1 : 0) - (vLeft ? 1 : 0);
        if (v != 0) mvx = v;
        MoveX = mvx;
        MoveY = mv.y;

        SprintHeld = sprintAction.IsPressed();
        CrouchHeld = crouchAction.IsPressed();
        InteractHeld = interactAction.IsPressed();

        // Jump: keyboard/gamepad OR virtual button. Virtual rising edge feeds the buffer.
        if (vJump && !vJumpPrev) LastJumpPressedTime = Time.time;
        vJumpPrev = vJump;
        JumpHeld = jumpAction.IsPressed() || vJump;
    }
}
