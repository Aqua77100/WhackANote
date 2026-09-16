using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrackPopupDisplay : MonoBehaviour
{
    [Tooltip("Must exactly match this track's LevelName on its StartGame script")]
    [SerializeField] private string trackId;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Image[] starImages; // assign Star 1, Star 2, Star 3 in order

    [Header("Star thresholds (adjust as needed)")]
    [SerializeField] private int oneStarThreshold = 500;
    [SerializeField] private int twoStarThreshold = 1500;
    [SerializeField] private int threeStarThreshold = 3000;

    [SerializeField] private Color filledStarColor = Color.white;
    [SerializeField] private Color emptyStarColor = new Color(1, 1, 1, 0.3f);

    private async void OnEnable()
    {
        string key = $"highscore_{trackId}";
        var loaded = await CloudSaveManager.Instance.LoadData(new HashSet<string> { key });

        int savedHighScore = loaded.ContainsKey(key) ? System.Convert.ToInt32(loaded[key]) : 0;

        if (scoreText != null)
        {
            scoreText.text = savedHighScore.ToString();
        }

        UpdateStars(savedHighScore);
    }

    private void UpdateStars(int score)
    {
        int starsEarned = 0;
        if (score >= threeStarThreshold) starsEarned = 3;
        else if (score >= twoStarThreshold) starsEarned = 2;
        else if (score >= oneStarThreshold) starsEarned = 1;

        for (int i = 0; i < starImages.Length; i++)
        {
            if (starImages[i] != null)
            {
                starImages[i].color = (i < starsEarned) ? filledStarColor : emptyStarColor;
            }
        }
    }
}