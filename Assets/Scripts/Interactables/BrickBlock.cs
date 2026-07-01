using UnityEngine;

/// <summary>Brick: a big player shatters it; a small player just bumps it.</summary>
public class BrickBlock : BumpableBlock
{
    [SerializeField] private int shatterScore = 50;
    [SerializeField] private AudioClip breakSfx;
    [SerializeField] private GameObject debrisPrefab;

    public override void OnBumpFromBelow(PlayerController player)
    {
        if (player != null && player.IsBig)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(breakSfx);
            if (GameManager.Instance != null) GameManager.Instance.AddScore(shatterScore);
            if (debrisPrefab != null) Instantiate(debrisPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else
        {
            base.OnBumpFromBelow(player);
        }
    }
}
