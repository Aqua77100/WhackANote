using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ScoreManager : MonoBehaviour
{

    public static ScoreManager Instance
    {
        get;
        private set;
    }

    public TextMeshProUGUI scoreText;
    private int currentScore = 0;
    private int _cachedHighScore = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (scoreText == null)
        {
            scoreText = Object.FindAnyObjectByType<TextMeshProUGUI>();
        }

        // Covers the case where GameManager already finished loading before this runs
        if (GameManager.Instance != null)
        {
            _cachedHighScore = GameManager.Instance.RestoredHighScore;
        }

        // Covers the case where GameManager finishes loading AFTER this runs
        GameManager.OnHighScoreRestored += HandleHighScoreRestored;
    }

    private void OnDestroy()
    {
        GameManager.OnHighScoreRestored -= HandleHighScoreRestored;
    }

    private void HandleHighScoreRestored(int restoredScore)
    {
        _cachedHighScore = restoredScore;
        Debug.Log($"ScoreManager: high score updated from restore event: {restoredScore}");
    }

    public void AddScore(int points)
    {
        currentScore += points;

        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }
    }

    public int GetScore()
    {
        return currentScore;
    }

    public async Task SaveHighScoreIfBeaten()
    {
        var loaded = await CloudSaveManager.Instance.LoadData(new HashSet<string> { "high_score" });
        int savedHighScore = loaded.ContainsKey("high_score") ? System.Convert.ToInt32(loaded["high_score"]) : 0;

        if (currentScore > savedHighScore)
        {
            var data = new Dictionary<string, object> { { "high_score", currentScore } };
            await CloudSaveManager.Instance.SaveData(data);
            Debug.Log($"New high score saved: {currentScore}");
        }
        else
        {
            Debug.Log($"Score {currentScore} did not beat saved high score {savedHighScore}");
        }
    }

    public int GetSavedHighScore()
    {
        return _cachedHighScore;
    }

}