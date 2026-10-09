using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SavedLevelData
{
    public string levelName;
    public string backgroundName;
    public int bpm;
    public int[] sequence;
}

public class LevelSavePopup : MonoBehaviour
{
    [Header("References")]
    public Composer composer;
    public GameObject popupPanel;

    [Header("Input Fields")]
    public TMP_InputField levelNameInput;
    public TMP_Dropdown backgroundDropdown;
    public TMP_Dropdown bpmDropdown;
    public TMP_Text statusText;

    [Header("Level Image Preview")]
    public Image levelImagePreview;
    public Sprite[] backgroundSprites;

    [Header("Background UI")]
    public CanvasGroup mainUIGroup;
    public GameObject dimBackground;

    private void Start()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);

        if (backgroundDropdown != null)
        {
            backgroundDropdown.onValueChanged.AddListener(UpdateBackgroundPreview);
            UpdateBackgroundPreview(backgroundDropdown.value);
        }

        if (dimBackground != null)
            dimBackground.SetActive(false);
    }

    public void OpenPopup()
    {
        if (composer == null)
        {
            Debug.LogError("Composer reference is missing.");
            return;
        }

        if (composer.IsRecording)
            composer.StopRecording();

        // Disable the normal UI buttons.
        if (mainUIGroup != null)
        {
            mainUIGroup.interactable = false;
            mainUIGroup.blocksRaycasts = false;
        }

        // Darken the screen.
        if (dimBackground != null)
            dimBackground.SetActive(true);

        // Show the popup on top.
        if (popupPanel != null)
            popupPanel.SetActive(true);

        if (statusText != null)
            statusText.text = "";

        if (levelNameInput != null &&
            string.IsNullOrWhiteSpace(levelNameInput.text))
        {
            levelNameInput.text = "My Level";
        }

        SetBpmDropdownToCurrent();
    }

    public void ClosePopup()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);

        // Restore the normal UI buttons.
        if (mainUIGroup != null)
        {
            mainUIGroup.interactable = true;
            mainUIGroup.blocksRaycasts = true;
        }

        // Remove the dark overlay.
        if (dimBackground != null)
            dimBackground.SetActive(false);
    }

    private void SetBpmDropdownToCurrent()
    {
        if (bpmDropdown == null || composer == null)
            return;

        int currentBPM = Mathf.RoundToInt(composer.playbackBPM);

        for (int i = 0; i < bpmDropdown.options.Count; i++)
        {
            string optionText = bpmDropdown.options[i].text
                .Replace("BPM", "").Trim();

            if (int.TryParse(optionText, out int optionBPM) &&
                optionBPM == currentBPM)
            {
                bpmDropdown.value = i;
                bpmDropdown.RefreshShownValue();
                return;
            }
        }
    }

    public void SaveLevel()
    {
        if (composer == null ||
            levelNameInput == null ||
            backgroundDropdown == null ||
            bpmDropdown == null)
        {
            ShowStatus("Some Inspector references are missing.");
            return;
        }

        string levelName = levelNameInput.text.Trim();

        if (string.IsNullOrWhiteSpace(levelName))
        {
            ShowStatus("Please enter a level name.");
            return;
        }

        if (composer.IsRecording)
            composer.StopRecording();

        int[] sequence = composer.GetSequence();

        if (sequence == null || sequence.Length == 0)
        {
            ShowStatus("Record a sequence before saving.");
            return;
        }

        if (backgroundDropdown.options.Count == 0 ||
            bpmDropdown.options.Count == 0)
        {
            ShowStatus("Please configure both dropdowns.");
            return;
        }

        string backgroundName =
            backgroundDropdown.options[
                backgroundDropdown.value
            ].text;

        string bpmText = bpmDropdown.options[
            bpmDropdown.value
        ].text.Replace("BPM", "").Trim();

        if (!int.TryParse(bpmText, out int selectedBPM) ||
            selectedBPM <= 0)
        {
            ShowStatus("Please select a valid BPM.");
            return;
        }

        string safeName = MakeSafeFileName(levelName);

        if (string.IsNullOrWhiteSpace(safeName))
        {
            ShowStatus("Please choose a valid level name.");
            return;
        }

        string directory = Path.Combine(
            Application.persistentDataPath,
            "CreatedLevels"
        );

        Directory.CreateDirectory(directory);

        string filePath = Path.Combine(
            directory,
            safeName + ".json"
        );

        if (File.Exists(filePath))
        {
            ShowStatus(
                "A level with that name already exists. " +
                "Choose another name."
            );
            return;
        }

        SavedLevelData data = new SavedLevelData
        {
            levelName = levelName,
            backgroundName = backgroundName,
            bpm = selectedBPM,
            sequence = sequence
        };

        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(filePath, json);

        // Apply the selected BPM to your existing preview.
        composer.SetBPM(selectedBPM);

        ShowStatus("Level saved successfully!");

        Debug.Log("Level saved to: " + filePath);
    }

    private string MakeSafeFileName(string value)
    {
        foreach (char invalidChar in
                 Path.GetInvalidFileNameChars())
        {
            value = value.Replace(invalidChar, '_');
        }

        return value.Trim();
    }

    private void ShowStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;

        Debug.Log(message);
    }

    public void UpdateBackgroundPreview(int index)
    {
        if (levelImagePreview == null ||
            backgroundSprites == null ||
            index < 0 ||
            index >= backgroundSprites.Length)
        {
            return;
        }

        levelImagePreview.sprite = backgroundSprites[index];
        levelImagePreview.enabled = backgroundSprites[index] != null;
    }
}
