using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Authoritative game state, persistent across level reloads so lives/score
/// survive a respawn. Drives death/respawn/game-over and level-complete flow.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int startingLives = 3;
    [SerializeField] private int startingTime = 300;
    [SerializeField] private int coinScoreValue = 200;
    [SerializeField] private int timeBonusPerSecond = 50;
    [SerializeField] private float deathDelay = 2f;
    [SerializeField] private float gameOverDelay = 3f;
    [SerializeField] private float levelCompleteDelay = 4f;

    public int Score { get; private set; }
    public int Coins { get; private set; }
    public int Lives { get; private set; }
    public int TimeRemaining { get; private set; }

    public enum State { Playing, Dead, LevelComplete, GameOver }
    public State CurrentState { get; private set; }

    private float timeAccumulator;
    private bool initialized;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (!initialized)
        {
            Score = 0; Coins = 0; Lives = startingLives;
            initialized = true;
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        if (Instance == this) SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start() { InitLevel(); }

    private void OnSceneLoaded(Scene s, LoadSceneMode mode) { InitLevel(); }

    private void InitLevel()
    {
        TimeRemaining = startingTime;
        timeAccumulator = 0f;
        CurrentState = State.Playing;
        BroadcastAll();
    }

    private void BroadcastAll()
    {
        GameEvents.ScoreChanged(Score);
        GameEvents.CoinsChanged(Coins);
        GameEvents.LivesChanged(Lives);
        GameEvents.TimeChanged(TimeRemaining);
    }

    void Update()
    {
        if (CurrentState != State.Playing) return;
        timeAccumulator += Time.deltaTime;
        while (timeAccumulator >= 1f)
        {
            timeAccumulator -= 1f;
            TimeRemaining = Mathf.Max(0, TimeRemaining - 1);
            GameEvents.TimeChanged(TimeRemaining);
            if (TimeRemaining <= 0) { RegisterPlayerDeath(); break; }
        }
    }

    public void AddScore(int amount) { Score += amount; GameEvents.ScoreChanged(Score); }

    public void AddCoin(int amount = 1)
    {
        Coins += amount;
        AddScore(coinScoreValue * amount);
        while (Coins >= 100) { Coins -= 100; AddLife(); }
        GameEvents.CoinsChanged(Coins);
    }

    public void AddLife() { Lives++; GameEvents.LivesChanged(Lives); }

    public void RegisterPlayerDeath()
    {
        if (CurrentState != State.Playing) return;
        GameEvents.PlayerDied();
        Lives = Mathf.Max(0, Lives - 1);
        GameEvents.LivesChanged(Lives);
        CurrentState = State.Dead;
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        yield return new WaitForSecondsRealtime(deathDelay);
        if (Lives > 0)
        {
            ReloadLevel();
        }
        else
        {
            CurrentState = State.GameOver;
            GameEvents.GameOver();
            yield return new WaitForSecondsRealtime(gameOverDelay);
            Score = 0; Coins = 0; Lives = startingLives;
            ReloadLevel();
        }
    }

    public void CompleteLevel()
    {
        if (CurrentState != State.Playing) return;
        CurrentState = State.LevelComplete;
        AddScore(TimeRemaining * timeBonusPerSecond);
        GameEvents.LevelComplete();
        StartCoroutine(LevelCompleteSequence());
    }

    private IEnumerator LevelCompleteSequence()
    {
        yield return new WaitForSecondsRealtime(levelCompleteDelay);
        ReloadLevel();
    }

    private void ReloadLevel()
    {
        GameEvents.Respawn();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
