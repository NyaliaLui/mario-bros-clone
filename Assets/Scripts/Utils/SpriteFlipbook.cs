using UnityEngine;

/// <summary>
/// Lightweight looping sprite animation: cycles through a frame array at a
/// given fps on the attached SpriteRenderer. Call Play() to switch frame sets.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteFlipbook : MonoBehaviour
{
    [SerializeField] private Sprite[] frames;
    [SerializeField] private float fps = 8f;
    [SerializeField] private bool loop = true;
    [SerializeField] private bool playOnEnable = true;

    private SpriteRenderer sr;
    private float timer;
    private int index;
    private bool playing;

    public bool IsPlaying => playing;

    void Awake() { sr = GetComponent<SpriteRenderer>(); }

    void OnEnable()
    {
        if (playOnEnable && frames != null && frames.Length > 0) Play(frames);
    }

    public void Play(Sprite[] newFrames)
    {
        frames = newFrames;
        index = 0;
        timer = 0f;
        playing = frames != null && frames.Length > 0;
        if (playing && sr != null) sr.sprite = frames[0];
    }

    public void Stop() { playing = false; }

    public void SetFps(float value) { fps = value; }

    void Update()
    {
        if (!playing || frames == null || frames.Length <= 1 || fps <= 0f) return;
        timer += Time.deltaTime;
        float frameTime = 1f / fps;
        while (timer >= frameTime)
        {
            timer -= frameTime;
            index++;
            if (index >= frames.Length)
            {
                if (loop) index = 0;
                else { index = frames.Length - 1; playing = false; break; }
            }
        }
        if (sr != null) sr.sprite = frames[index];
    }
}
