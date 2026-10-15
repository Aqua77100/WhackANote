
using UnityEngine;
using UnityEngine.UI;

public class ThemeColour : MonoBehaviour
{
    [Header("Theme Colours")]
    [SerializeField] private Color lightModeColour = Color.white;
    [SerializeField] private Color darkModeColour = Color.black;

    private Graphic uiGraphic;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        // For UI Images and TextMeshPro UI text
        uiGraphic = GetComponent<Graphic>();

        // For regular 2D sprites
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        ThemeManager.OnThemeChanged += ApplyTheme;

        // Apply the currently saved theme immediately
        ApplyTheme();
    }

    private void OnDisable()
    {
        ThemeManager.OnThemeChanged -= ApplyTheme;
    }

    private void ApplyTheme()
    {
        Color selectedColour = ThemeManager.IsDarkMode
            ? darkModeColour
            : lightModeColour;

        if (uiGraphic != null)
        {
            uiGraphic.color = selectedColour;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = selectedColour;
        }
    }
}
