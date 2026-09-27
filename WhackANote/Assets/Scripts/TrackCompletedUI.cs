using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrackCompletedUI : MonoBehaviour
{
    [Header("Hit Counts")]
    [SerializeField] private TextMeshProUGUI perfectsText;
    [SerializeField] private TextMeshProUGUI greatsText;
    [SerializeField] private TextMeshProUGUI goodsText;
    [SerializeField] private TextMeshProUGUI missesText;

    [Header("Scores")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private GameObject newHighScoreBadge;

    [Header("Stars")]
    [SerializeField] private Image[] starImages; // Assign 3 star images
    [SerializeField] private Sprite starFullSprite;
    [SerializeField] private Sprite starHalfSprite;
    [SerializeField] private Sprite starEmptySprite;

    public void DisplayStats(TrackCompletionStats stats)
    {
        // --- Hit Types ---
        if (perfectsText != null) perfectsText.text = stats.Perfects.ToString();
        if (greatsText != null)   greatsText.text = stats.Greats.ToString();
        if (goodsText != null)    goodsText.text = stats.Goods.ToString();
        if (missesText != null)   missesText.text = stats.Misses.ToString();

        // --- Scores ---
        if (scoreText != null)    scoreText.text = $"Score: {stats.CurrentScore:N0}";
        if (highScoreText != null) highScoreText.text = $"High Score: {stats.HighScore:N0}";

        
        if (newHighScoreBadge != null)
        {
            newHighScoreBadge.SetActive(stats.IsNewHighScore());
        }

        // Apply Half / Full Star sprites
        StarState[] starStates = stats.CalculateStarStates();
        for (int i = 0; i < starImages.Length && i < starStates.Length; i++)
        {
            switch (starStates[i])
            {
                case StarState.Full:  starImages[i].sprite = starFullSprite; break;
                case StarState.Half:  starImages[i].sprite = starHalfSprite; break;
                case StarState.Empty: starImages[i].sprite = starEmptySprite; break;
            }
        }
    }
}