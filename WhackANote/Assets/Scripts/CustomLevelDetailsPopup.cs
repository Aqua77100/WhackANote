
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

    public void Show(
        SavedLevelData data,
        string filePath,
        Sprite backgroundSprite)
    {
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

}
