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
        try
        {
            string key = $"highscore_{StartGame.CurrentTrackId}";

            var loaded = await CloudSaveManager.Instance.LoadData(new HashSet<string> { key });
            int savedHighScore = loaded.ContainsKey(key) ? System.Convert.ToInt32(loaded[key]) : 0;

            if (currentScore > savedHighScore)
            {
                var data = new Dictionary<string, object> { { key, currentScore } };
                await CloudSaveManager.Instance.SaveData(data);
                Debug.Log($"New high score saved for {StartGame.CurrentTrackId}: {currentScore}");
            }
            else
            {
                Debug.Log($"Score {currentScore} did not beat saved high score {savedHighScore} for {StartGame.CurrentTrackId}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public int GetSavedHighScore()
    {
        return _cachedHighScore;
    }

}