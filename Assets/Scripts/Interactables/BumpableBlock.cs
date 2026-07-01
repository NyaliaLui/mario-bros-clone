using System.Collections;
using UnityEngine;

/// <summary>Base for blocks the player bumps from below: plays a bump-and-settle tween.</summary>
[RequireComponent(typeof(Collider2D))]
public abstract class BumpableBlock : MonoBehaviour, IBumpable
{
    [SerializeField] protected AudioClip bumpSfx;
    [SerializeField] protected float bumpHeight = 0.25f;
    [SerializeField] protected float bumpDuration = 0.12f;

    protected bool isBumping;
    protected Vector3 restPos;

    protected virtual void Awake() { restPos = transform.position; }

    public virtual void OnBumpFromBelow(PlayerController player)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(bumpSfx);
        if (!isBumping) StartCoroutine(BumpTween());
    }

    protected IEnumerator BumpTween()
    {
        isBumping = true;
        float t = 0f;
        while (t < bumpDuration)
        {
            t += Time.deltaTime;
            float f = t / bumpDuration;
            transform.position = restPos + Vector3.up * (Mathf.Sin(f * Mathf.PI) * bumpHeight);
            yield return null;
        }
        transform.position = restPos;
        isBumping = false;
    }
}
