using UnityEngine;

/// <summary>Trigger volume (pits / below the level) that instantly kills the player.</summary>
[RequireComponent(typeof(Collider2D))]
public class KillZone : MonoBehaviour
{
    void Reset()
    {
        var c = GetComponent<Collider2D>();
        if (c != null) c.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        var hp = other.GetComponent<PlayerHealth>();
        if (hp == null) hp = other.GetComponentInParent<PlayerHealth>();
        if (hp != null) hp.Die(transform.position);
    }
}
