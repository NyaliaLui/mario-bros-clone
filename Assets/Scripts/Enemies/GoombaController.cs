using UnityEngine;

/// <summary>Goomba-like walker: stomped from above dies; side contact damages the player.</summary>
public class GoombaController : EnemyBase, IStompable
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (dead) return;
        if (!collision.collider.CompareTag("Player")) return;

        var player = collision.collider.GetComponent<PlayerController>();
        if (player == null) player = collision.collider.GetComponentInParent<PlayerController>();
        var prb = collision.collider.attachedRigidbody;

        bool fromAbove = collision.collider.bounds.min.y > col.bounds.center.y;
        bool movingDown = prb == null || prb.linearVelocity.y <= 1f;

        if (player != null && fromAbove && movingDown) OnStomped(player);
        else DamagePlayer(collision.collider);
    }

    public void OnStomped(PlayerController player)
    {
        if (dead) return;
        if (sr != null)
        {
            var t = sr.transform;
            t.localScale = new Vector3(t.localScale.x, t.localScale.y * 0.35f, 1f);
        }
        float bounce = config != null ? config.stompBounce : 12f;
        if (player != null) player.Bounce(bounce);
        Die();
    }
}
