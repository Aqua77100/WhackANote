
using System;
using UnityEngine;
using UnityEngine.UI;

public class CustomLevelBackground : MonoBehaviour
{
    [System.Serializable]
    public class BackgroundOption
    {
        public string backgroundName;
        public Sprite sprite;
    }

    [Header("Background Display")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private SpriteRenderer backgroundRenderer;

    [Header("Available Backgrounds")]
    public BackgroundOption[] backgrounds;

    public void SetBackground(string backgroundName)
    {
        foreach (BackgroundOption option in backgrounds)
        {
            if (option == null)
                continue;

            if (string.Equals(
                option.backgroundName?.Trim(),
                backgroundName?.Trim(),
                StringComparison.OrdinalIgnoreCase))
            {
                if (option.sprite == null)
                {
                    Debug.LogWarning(
                        "No sprite assigned for: " + backgroundName
                    );
                    return;
                }

                // Background displayed through a Canvas Image
                if (backgroundImage != null)
                {
                    backgroundImage.sprite = option.sprite;
                }

                // Background displayed through a SpriteRenderer
                if (backgroundRenderer != null)
                {
                    backgroundRenderer.sprite = option.sprite;
                }

                Debug.Log("Background changed to: " + backgroundName);
                return;
            }
        }

        Debug.LogWarning(
            "Could not find background: " + backgroundName
        );
    }
}
