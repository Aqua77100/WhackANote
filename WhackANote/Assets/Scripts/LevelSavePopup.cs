using System;
using System.IO;
using TMPro;
using UnityEngine;

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

    private void Start()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
    }

    public void OpenPopup()
    {
        if (composer == null)
        {
            Debug.LogError("Composer reference is missing.");
            return;
        }

        // Stop recording before opening the save menu.
        if (composer.IsRecording)
            composer.StopRecording();

        popupPanel.SetActive(true);

        if (statusText != null)
            statusText.text = "";

        if (levelNameInput != null &&
            string.IsNullOrWhiteSpace(levelNameInput.text))
        {
            levelNameInput.text = "My Level";
        }

        // Default to the current playback BPM.
        SetBpmDropdownToCurrent();
    }

    public void ClosePopup()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
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
}
