using UnityEngine;

/// <summary>
/// Drives a transform that the Cinemachine camera follows. Implements the
/// classic side-scroller rule: horizontal position only ever advances
/// (never scrolls back left), vertical stays fixed.
/// </summary>
public class CameraTargetFollower : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float fixedY = 3f;
    [SerializeField] private bool lockLeftEdge = true;

    private float maxX;

    void Start()
    {
        if (target != null) maxX = target.position.x;
        ApplyPosition();
    }

    void LateUpdate()
    {
        if (target == null) return;
        float x = target.position.x;
        if (lockLeftEdge) { x = Mathf.Max(maxX, x); maxX = x; }
        ApplyPosition(x);
    }

    private void ApplyPosition() { if (target != null) ApplyPosition(target.position.x); }
    private void ApplyPosition(float x) { transform.position = new Vector3(x, fixedY, transform.position.z); }

    public void SetTarget(Transform t) { target = t; if (t != null) maxX = t.position.x; }
}
