
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomLevelCard : MonoBehaviour
{
    [Header("Card References")]
    public TMP_Text titleText;
    public Image backgroundImage;

    private SavedLevelData levelData;
    private string levelFilePath;
    private Sprite levelSprite;

    private Action<SavedLevelData, string, Sprite> onSelected;

    // Called when the manager creates this card
    public void Setup(
        SavedLevelData data,
        string filePath,
        Sprite sprite,
        Action<SavedLevelData, string, Sprite> callback)
    {
        levelData = data;
        levelFilePath = filePath;
        levelSprite = sprite;
        onSelected = callback;

        // Update the card's title
        if (titleText != null)
            titleText.text = data.levelName;

        // Update the card's image
        if (backgroundImage != null)
            backgroundImage.sprite = sprite;
    }

    // Called by the Play button's On Click()
    public void OpenDetailsPopup()
    {
        if (levelData == null)
        {
            Debug.LogError("No level data assigned to this card.");
            return;
        }

        Debug.Log("Opening details for: " + levelData.levelName);

        onSelected?.Invoke(
            levelData,
            levelFilePath,
            levelSprite
        );
    }
}
