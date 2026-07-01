using UnityEngine;

/// <summary>A coin that pops out of a bumped block: arcs up, credits a coin, self-destructs.</summary>
public class CoinPop : MonoBehaviour
{
    [SerializeField] private float riseHeight = 1.2f;
    [SerializeField] private float duration = 0.45f;
    [SerializeField] private AudioClip sfx;

    private float t;
    private Vector3 start;

    void Start()
    {
        start = transform.position;
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(sfx);
    }

    void Update()
    {
        t += Time.deltaTime;
        float f = t / duration;
        transform.position = start + Vector3.up * (Mathf.Sin(f * Mathf.PI) * riseHeight);
        if (f >= 1f) Destroy(gameObject);
    }
}
