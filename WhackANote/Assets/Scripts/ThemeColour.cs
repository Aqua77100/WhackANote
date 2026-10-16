
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ThemeColour : MonoBehaviour
{
    [Header("Theme Colours")]
    [SerializeField] private Color lightModeColour = Color.white;
    [SerializeField] private Color darkModeColour = Color.white;

    [Header("Optional Sprite Variants")]
    [SerializeField] private Sprite lightModeSprite;
    [SerializeField] private Sprite darkModeSprite;

    [Header("Optional Dyslexia Font Colours")]
    [SerializeField] private bool overrideDyslexiaColour = false;
    [SerializeField] private Color dyslexiaLightColour = Color.black;
    [SerializeField] private Color dyslexiaDarkColour = Color.white;

    private Graphic uiGraphic;
    private Image uiImage;
    private SpriteRenderer spriteRenderer;

    private Sprite originalUISprite;
    private Sprite originalWorldSprite;

    private void Awake()
    {
        uiGraphic = GetComponent<Graphic>();
        uiImage = GetComponent<Image>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Remember the original sprites as fallbacks
        if (uiImage != null)
            originalUISprite = uiImage.sprite;

        if (spriteRenderer != null)
            originalWorldSprite = spriteRenderer.sprite;
    }

    private void OnEnable()
    {
        ThemeManager.OnThemeChanged += ApplyTheme;
        ApplyTheme();
    }

    private void OnDisable()
    {
        ThemeManager.OnThemeChanged -= ApplyTheme;
    }

    private void ApplyTheme()
    {
        bool isDark = ThemeManager.IsDarkMode;

        // 1. Select colour
        Color selectedColour = isDark
            ? darkModeColour
            : lightModeColour;

        // 2. Select optional sprite
        Sprite selectedSprite = isDark
            ? darkModeSprite
            : lightModeSprite;

        // 3. Apply to UI Images
        if (uiImage != null)
        {
            uiImage.sprite = selectedSprite != null
                ? selectedSprite
                : originalUISprite;
        }

        // 4. Apply to UI graphics (Images and TMP text)
        if (uiGraphic != null)
        {
            bool dyslexiaActive =
                uiGraphic is TMP_Text &&
                DyslexiaFontManager.Instance != null &&
                DyslexiaFontManager.Instance.IsDyslexiaFontEnabled();

            if (dyslexiaActive && overrideDyslexiaColour)
            {
                uiGraphic.color = isDark
                    ? dyslexiaDarkColour
                    : dyslexiaLightColour;
            }
            else
            {
                uiGraphic.color = selectedColour;
            }
        }

        // 5. Apply to 2D SpriteRenderers
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = selectedSprite != null
                ? selectedSprite
                : originalWorldSprite;

            spriteRenderer.color = selectedColour;
        }
    }

    public void RefreshTheme()
    {
        ApplyTheme();
    }
}
