
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CustomLevelDetailsPopup : MonoBehaviour
{
    [Header("Popup References")]
    public TMP_Text titleText;
    public Image backgroundPreview;
    public TMP_Text bpmText;
    public TMP_Text highScoreValueText;

    [Header("Optional Background Overlay")]
    public GameObject dimBackground;

    private string selectedLevelFileName;

    [Header("Gameplay")]
    public string gameplaySceneName = "CustomLevelGameplay";

    [Header("Delete Level")]
    public GameObject deleteConfirmationPanel;
    public CustomLevelSelectManager levelSelectManager;


    [Header("Star Rating")]
    [SerializeField] private Image[] starImages;

    [SerializeField] private Sprite starFullSprite;
    [SerializeField] private Sprite starHalfSprite;
    [SerializeField] private Sprite starNoneSprite;


    public void Show(SavedLevelData data, string filePath, Sprite backgroundSprite)
    {
        // panel is hidden whenever you open a diff level
        if (deleteConfirmationPanel != null)
        {
            deleteConfirmationPanel.SetActive(false);
        }

        if (data == null)
        {
            Debug.LogError("No level data was provided.");
            return;
        }

        selectedLevelFileName = Path.GetFileName(filePath);

        // Display the selected level's title.
        if (titleText != null)
            titleText.text = data.levelName;

        // Display its background image.
        if (backgroundPreview != null)
        {
            backgroundPreview.sprite = backgroundSprite;
            backgroundPreview.enabled = backgroundSprite != null;
        }

        // Display its saved BPM.
        if (bpmText != null)
            bpmText.text = data.bpm + " BPM";

        // Display this level's personal best.
        string scoreKey = "CustomLevelHighScore_" +
            Path.GetFileNameWithoutExtension(filePath);

        int bestScore = PlayerPrefs.GetInt(scoreKey, 0);

        if (highScoreValueText != null)
        {
            highScoreValueText.text =
                bestScore > 0 ? bestScore.ToString() : "-";
        }

        // Update stars based on this level's personal best
        UpdateStars(bestScore, data.sequence);

        if (dimBackground != null)
            dimBackground.SetActive(true);

        gameObject.SetActive(true);

        Debug.Log("Opened details for: " + data.levelName);
    }

    public void Close()
    {
        if (dimBackground != null)
            dimBackground.SetActive(false);

        gameObject.SetActive(false);
    }


    public void PlaySelectedLevel()
    {
        if (string.IsNullOrEmpty(selectedLevelFileName))
        {
            Debug.LogError("No custom level selected!");
            return;
        }

        if (string.IsNullOrWhiteSpace(gameplaySceneName))
        {
            Debug.LogError("Gameplay scene name is missing!");
            return;
        }

        // Check that the selected level still exists.
        string filePath = Path.Combine(
            Application.persistentDataPath,
            "CreatedLevels",
            selectedLevelFileName
        );

        if (!File.Exists(filePath))
        {
            Debug.LogError("Level file not found: " + filePath);
            return;
        }

        // Remember which level the player wants to play.
        PlayerPrefs.SetString(
            "SelectedCreatedLevelFile",
            selectedLevelFileName
        );

        PlayerPrefs.Save();

        Debug.Log("Launching custom level: " + selectedLevelFileName);

        // Open the shared gameplay scene.
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void ShowDeleteConfirmation()
    {
        if(deleteConfirmationPanel != null)
        {
            deleteConfirmationPanel.SetActive(true);
        }
    }

    public void CancelDelete()
    {
        if (deleteConfirmationPanel != null)
        {
            deleteConfirmationPanel.SetActive(false);
        }
    }

    public void ConfirmDelete()
    {
        if (string.IsNullOrEmpty(selectedLevelFileName))
        {
            Debug.LogError("No custom level selected for deletion!");
            return;
        }

        // Get the saved JSON file's location.
        string filePath = Path.Combine(
            Application.persistentDataPath,
            "CreatedLevels",
            Path.GetFileName(selectedLevelFileName)
        );

        // Only allow saved JSON level files to be deleted.
        if (!string.Equals(
            Path.GetExtension(filePath),
            ".json",
            System.StringComparison.OrdinalIgnoreCase))
        {
            Debug.LogError("Invalid custom level file!");
            return;
        }

        if (!File.Exists(filePath))
        {
            Debug.LogError("Level file does not exist: " + filePath);
            return;
        }

        try
        {
            // Permanently delete the saved level.
            File.Delete(filePath);
            
            string scoreKey = "CustomLevelHighScore_" +
            Path.GetFileNameWithoutExtension(filePath);

            PlayerPrefs.DeleteKey(scoreKey);
            PlayerPrefs.Save();

            Debug.Log("Deleted custom level: " + selectedLevelFileName);

            selectedLevelFileName = null;

            // Close the details popup.
            Close();

            // Refresh the cards to remove the deleted level.
            if (levelSelectManager != null)
            {
                levelSelectManager.LoadSavedLevels();
            }
            else
            {
                Debug.LogWarning("Level Select Manager is not assigned!");
            }
        }
        catch (System.Exception exception)
        {
            Debug.LogError("Failed to delete level: " + exception.Message);
        }
    }


    private void UpdateStars(int score, int[] sequence)
    {
        if (sequence == null)
            return;

        // Count playable notes (ignore rests represented by -1)
        int playableNotes = 0;

        foreach (int note in sequence)
        {
            if (note >= 0)
            {
                playableNotes++;
            }
        }

        // Each PERFECT hit is worth 50 points
        int maximumScore = playableNotes * 50;

        if (maximumScore <= 0)
        {
            foreach (Image star in starImages)
            {
                if (star != null)
                    star.sprite = starNoneSprite;
            }

            return;
        }

        // Calculate thresholds based on the maximum score
        int oneStarThreshold = Mathf.CeilToInt(maximumScore * 0.30f);
        int twoStarThreshold = Mathf.CeilToInt(maximumScore * 0.60f);
        int threeStarThreshold = Mathf.CeilToInt(maximumScore * 0.90f);

        int[] thresholds =
        {
            0,
            oneStarThreshold,
            twoStarThreshold,
            threeStarThreshold
        };

        for (int i = 0; i < starImages.Length && i < 3; i++)
        {
            if (starImages[i] == null)
                continue;

            int lowerBound = thresholds[i];
            int upperBound = thresholds[i + 1];
            int halfwayPoint =
                lowerBound + (upperBound - lowerBound) / 2;

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
