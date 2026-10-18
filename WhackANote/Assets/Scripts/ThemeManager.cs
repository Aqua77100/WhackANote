
using UnityEngine;
using UnityEngine.UI;
using System;

public class ThemeManager : MonoBehaviour
{
    [Header("Theme Settings")]
    [SerializeField] private Toggle darkModeToggle;

    private const string ThemeKey = "DarkMode";

    public static event Action OnThemeChanged;

    // Check the player's saved preference
    public static bool IsDarkMode =>
        PlayerPrefs.GetInt(ThemeKey, 0) == 1;

    private void OnEnable()
    {
        if (darkModeToggle != null)
        {
            // Load the saved toggle state
            darkModeToggle.SetIsOnWithoutNotify(IsDarkMode);

            darkModeToggle.onValueChanged.AddListener(SetDarkMode);
        }
    }

    private void OnDisable()
    {
        if (darkModeToggle != null)
        {
            darkModeToggle.onValueChanged.RemoveListener(SetDarkMode);
        }
    }

    public void SetDarkMode(bool enabled)
    {
        // Save the player's preference
        PlayerPrefs.SetInt(ThemeKey, enabled ? 1 : 0);
        PlayerPrefs.Save();

        // Tell other objects to update their colours
        OnThemeChanged?.Invoke();
    }
}
