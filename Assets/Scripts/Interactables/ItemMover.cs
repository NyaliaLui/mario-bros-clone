using System.Collections;
using UnityEngine;

/// <summary>
/// Shared item behaviour: emerge from a block (rise while non-colliding),
/// then walk horizontally with gravity, reversing at walls.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class ItemMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float emergeHeight = 1f;
    [SerializeField] private float emergeDuration = 0.5f;
    [SerializeField] private int startDir = 1;

    private Rigidbody2D rb;
    private Collider2D col;
    private int dir;
    private bool emerged;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        dir = startDir;
    }

    void Start() { StartCoroutine(Emerge()); }

    private IEnumerator Emerge()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        if (col != null) col.enabled = false;
        Vector3 s = transform.position;
        Vector3 e = s + Vector3.up * emergeHeight;
        float t = 0f;
        while (t < emergeDuration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(s, e, t / emergeDuration);
            yield return null;
        }
        transform.position = e;
        if (col != null) col.enabled = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 2f;
        rb.freezeRotation = true;
        emerged = true;
    }

    void FixedUpdate()
    {
        if (!emerged) return;
        rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        if (!emerged) return;
        for (int i = 0; i < c.contactCount; i++)
        {
            if (Mathf.Abs(c.GetContact(i).normal.x) > 0.5f) { dir = -dir; break; }
        }
    }
}
