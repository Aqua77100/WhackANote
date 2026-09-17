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

    [Header("Star sprites")]
    [SerializeField] private Sprite starFullSprite;
    [SerializeField] private Sprite starHalfSprite;
    [SerializeField] private Sprite starNoneSprite;

    [Header("Star thresholds (adjust as needed)")]
    [SerializeField] private int oneStarThreshold = 500;
    [SerializeField] private int twoStarThreshold = 1500;
    [SerializeField] private int threeStarThreshold = 3000;

    private async void OnEnable()
    {
        try
        {
            string key = $"highscore_{trackId}";
            var loaded = await CloudSaveManager.Instance.LoadData(new HashSet<string> { key });

            int savedHighScore = loaded.ContainsKey(key) ? System.Convert.ToInt32(loaded[key]) : 0;

            Debug.Log($"TrackPopupDisplay ({trackId}): loaded high score = {savedHighScore}");

            if (scoreText != null)
            {
                scoreText.text = savedHighScore.ToString();
            }

            UpdateStars(savedHighScore);
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    private void UpdateStars(int score)
    {
        int[] thresholds = { 0, oneStarThreshold, twoStarThreshold, threeStarThreshold };

        for (int i = 0; i < starImages.Length; i++)
        {
            if (starImages[i] == null) continue;

            int lowerBound = thresholds[i];       // where this star starts
            int upperBound = thresholds[i + 1];    // where this star is fully earned
            int halfwayPoint = lowerBound + (upperBound - lowerBound) / 2;

            if (score >= upperBound)
            {
                starImages[i].sprite = starFullSprite;
            }
            else if (score >= halfwayPoint)
            {
                starImages[i].sprite = starHalfSprite;
            }
            else
            {
                starImages[i].sprite = starNoneSprite;
            }
        }
    }
}