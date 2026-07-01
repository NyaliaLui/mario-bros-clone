using System;

/// <summary>
/// Static event hub. GameManager raises these; UI/audio subscribe. Keeps
/// gameplay state decoupled from presentation.
/// </summary>
public static class GameEvents
{
    public static event Action<int> OnScoreChanged;
    public static event Action<int> OnCoinsChanged;
    public static event Action<int> OnLivesChanged;
    public static event Action<int> OnTimeChanged;
    public static event Action OnPlayerDied;
    
    public static event Action OnGameOver;
    public static event Action OnRespawn;
public static event Action OnLevelComplete;

    public static void ScoreChanged(int v) { if (OnScoreChanged != null) OnScoreChanged(v); }
    public static void CoinsChanged(int v) { if (OnCoinsChanged != null) OnCoinsChanged(v); }
    public static void LivesChanged(int v) { if (OnLivesChanged != null) OnLivesChanged(v); }
    public static void TimeChanged(int v) { if (OnTimeChanged != null) OnTimeChanged(v); }
    public static void PlayerDied() { if (OnPlayerDied != null) OnPlayerDied(); }
    
    public static void GameOver() { if (OnGameOver != null) OnGameOver(); }
    public static void Respawn() { if (OnRespawn != null) OnRespawn(); }
public static void LevelComplete() { if (OnLevelComplete != null) OnLevelComplete(); }
}
