using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;

public class ScoreManager : MonoBehaviour
{

    public static ScoreManager Instance { get; private set; }

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI multiplierText;
    
    private int currentScore = 0;
    private int _cachedHighScore = 0;

    [Header("Perfect Streak Settings")]
    public int perfectStreakThreshold = 10;
    public float perfectMultiplier = 1.05f;
    private int currentPerfectStreak = 0;

    [Header("Game Over Settings")]
    [Tooltip("Enable or disable failing from missing too many moles (great for testing!)")]
    public bool enableGameOverOnMisses = true;
    public int maxAllowedMisses = 10;

    // --- HIT COUNT & MULTIPLIER TRACKING ---
    public int Perfects { get; private set; }
    public int Greats { get; private set; }
    public int Goods { get; private set; }
    public int Misses { get; private set; }
    public int MultiplierCount { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        ResetStatsOnScreenLoad();

        if (scoreText == null)
        {
            scoreText = Object.FindAnyObjectByType<TextMeshProUGUI>();
        }

        if (multiplierText == null)
        {
            multiplierText = Object.FindAnyObjectByType<TextMeshProUGUI>();
        }
        multiplierText.gameObject.SetActive(false);

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

    public void ResetStatsOnScreenLoad()
    {
        currentScore = 0;
        Perfects = 0;
        Greats = 0;
        Goods = 0;
        Misses = 0;
        MultiplierCount = 0;
    }

    public void AddScore(int points, string hitType = "")
    {
        // Increment hit types based on the feedback string
        switch (hitType)
        {
            case "PERFECT!": Perfects++; currentPerfectStreak++; break;
            case "GREAT!":   Greats++; currentPerfectStreak = 0; break;
            case "GOOD!":    Goods++; currentPerfectStreak = 0; break;
            case "MISS":     
                Misses++; 
                currentPerfectStreak = 0;

                // Check for GameOver condition
                if (enableGameOverOnMisses && Misses >= maxAllowedMisses)
                {
                    if (PauseMenu.Instance != null)
                    {
                        PauseMenu.Instance.TriggerGameOver();
                    }
                    return; // Stop any  score processing
                }
                break;
        }

        currentScore += points;

        // Check for 10 Perfect hits in a row
        if (currentPerfectStreak >= perfectStreakThreshold)
        {
            // 1.05 x current score
            currentScore = Mathf.RoundToInt(currentScore * perfectMultiplier);

            // Reset streak so that player needs 10 fresh consecutive perfects for the next bonus
            currentPerfectStreak = 0;

            MultiplierCount++; 

            Debug.Log($"10 Perfect Streak hit! Score multiplied by {perfectMultiplier} to: {currentScore}");
            
            if (multiplierText != null)
            {
                StopAllCoroutines(); // Prevents overlapping timers if they hit another streak quickly (for fast levels)
                StartCoroutine(FlashMultiplierRoutine());
            }
        }

        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }
    }

    private IEnumerator FlashMultiplierRoutine()
    {
        multiplierText.gameObject.SetActive(true);
        
        // Set text color back to full opacity (alpha = 1)
        Color originalColor = multiplierText.color;
        multiplierText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);

        yield return new WaitForSeconds(1f);

        float fadeDuration = 3.0f;
        float currentTime = 0f;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            
            // Calculate the new alpha value (goes from 1 down to 0)
            float alpha = Mathf.Lerp(1f, 0f, currentTime / fadeDuration);
            
            // Apply the new alpha to the text color
            multiplierText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            
            yield return null; // Wait for the next frame
        }
        
        multiplierText.gameObject.SetActive(false);
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