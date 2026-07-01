using UnityEngine;
using UnityEngine.UI;

/// <summary>Subscribes to GameEvents and mirrors them onto on-screen text + panels.</summary>
public class HUDController : MonoBehaviour
{
    [SerializeField] private Text scoreText;
    [SerializeField] private Text coinsText;
    [SerializeField] private Text livesText;
    [SerializeField] private Text timeText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject levelCompletePanel;

    void OnEnable()
    {
        GameEvents.OnScoreChanged += SetScore;
        GameEvents.OnCoinsChanged += SetCoins;
        GameEvents.OnLivesChanged += SetLives;
        GameEvents.OnTimeChanged += SetTime;
        GameEvents.OnGameOver += ShowGameOver;
        GameEvents.OnLevelComplete += ShowLevelComplete;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
    }

    void OnDisable()
    {
        GameEvents.OnScoreChanged -= SetScore;
        GameEvents.OnCoinsChanged -= SetCoins;
        GameEvents.OnLivesChanged -= SetLives;
        GameEvents.OnTimeChanged -= SetTime;
        GameEvents.OnGameOver -= ShowGameOver;
        GameEvents.OnLevelComplete -= ShowLevelComplete;
    }

    private void SetScore(int v) { if (scoreText) scoreText.text = "SCORE\n" + v.ToString("D6"); }
    private void SetCoins(int v) { if (coinsText) coinsText.text = "COINS\n" + v.ToString("D2"); }
    private void SetLives(int v) { if (livesText) livesText.text = "LIVES\n" + v; }
    private void SetTime(int v) { if (timeText) timeText.text = "TIME\n" + v; }

    private void ShowGameOver() { if (gameOverPanel != null) gameOverPanel.SetActive(true); }
    private void ShowLevelComplete() { if (levelCompletePanel != null) levelCompletePanel.SetActive(true); }
}
