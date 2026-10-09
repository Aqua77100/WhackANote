
using System;
using System.IO;
using UnityEngine;

public class CustomLevelSelectManager : MonoBehaviour
{
    [System.Serializable]
    public class BackgroundOption
    {
        public string backgroundName;
        public Sprite sprite;
    }

    [Header("Card Setup")]
    public GameObject levelCardPrefab;
    public Transform cardContainer;

    [Header("Available Backgrounds")]
    public BackgroundOption[] backgrounds;

    public CustomLevelDetailsPopup detailsPopup;

    private void Start()
    {
        LoadSavedLevels();
    }

    public void LoadSavedLevels()
    {
        if (levelCardPrefab == null || cardContainer == null)
        {
            Debug.LogError(
                "Assign the card prefab and card container."
            );
            return;
        }

        string directory = Path.Combine(
            Application.persistentDataPath,
            "CreatedLevels"
        );

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
            Debug.Log("No saved custom levels found yet.");
            return;
        }

        // Remove previously generated cards before refreshing.
        for (int i = cardContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(cardContainer.GetChild(i).gameObject);
        }

        string[] files = Directory.GetFiles(directory, "*.json");
        Array.Sort(files, StringComparer.OrdinalIgnoreCase);

        foreach (string file in files)
        {
            try
            {
                string json = File.ReadAllText(file);

                SavedLevelData data =
                    JsonUtility.FromJson<SavedLevelData>(json);

                if (data == null ||
                    string.IsNullOrWhiteSpace(data.levelName) ||
                    data.sequence == null)
                {
                    Debug.LogWarning(
                        "Skipping invalid saved level: " + file
                    );
                    continue;
                }

                GameObject cardObject = Instantiate(
                    levelCardPrefab,
                    cardContainer
                );

                CustomLevelCard card =
                    cardObject.GetComponent<CustomLevelCard>();

                if (card == null)
                {
                    Debug.LogError(
                        "The card prefab needs a CustomLevelCard component."
                    );

                    Destroy(cardObject);
                    continue;
                }

                Sprite backgroundSprite =
                    FindBackground(data.backgroundName);

                card.Setup(
                    data,
                    file,
                    backgroundSprite,
                    HandleCardSelected
                );
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    "Could not load level file " + file +
                    ": " + exception.Message
                );
            }
        }

        Debug.Log("Loaded " + files.Length + " saved level file(s).");
    }

    private Sprite FindBackground(string backgroundName)
    {
        if (backgrounds == null)
            return null;

        foreach (BackgroundOption option in backgrounds)
        {
            if (option != null &&
                string.Equals(
                    option.backgroundName?.Trim(),
                    backgroundName?.Trim(),
                    StringComparison.OrdinalIgnoreCase))
            {
                return option.sprite;
            }
        }

        Debug.LogWarning(
            "No sprite mapping found for background: " + backgroundName
        );

        return null;
    }

    private void HandleCardSelected(SavedLevelData data,string filePath,Sprite backgroundSprite)
    {
        if (detailsPopup == null)
        {
            Debug.LogError("Custom level details popup is not assigned.");
            return;
        }

        detailsPopup.Show(data, filePath, backgroundSprite);
    }
}
