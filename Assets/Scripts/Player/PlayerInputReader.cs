using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Wraps the shared InputActionAsset and exposes plain fields so gameplay
/// scripts never touch Input System types directly.
/// </summary>
public class PlayerInputReader : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputAsset;
    [SerializeField] private string actionMapName = "Player";

    private InputActionMap map;
    private InputAction moveAction, jumpAction, sprintAction, crouchAction, interactAction;

    public float MoveX { get; private set; }
    public float MoveY { get; private set; }
    public bool JumpHeld { get; private set; }
    public bool SprintHeld { get; private set; }
    public bool CrouchHeld { get; private set; }
    public bool InteractHeld { get; private set; }

    /// <summary>Time.time of the most recent Jump press (for jump buffering).</summary>
    public float LastJumpPressedTime { get; private set; } = -999f;

    public void ConsumeJump() { LastJumpPressedTime = -999f; }

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
        MoveX = mv.x;
        MoveY = mv.y;
        JumpHeld = jumpAction.IsPressed();
        SprintHeld = sprintAction.IsPressed();
        CrouchHeld = crouchAction.IsPressed();
        InteractHeld = interactAction.IsPressed();
    }
}
