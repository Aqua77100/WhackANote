using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DyslexiaFontManager : MonoBehaviour
{
    public static DyslexiaFontManager Instance { get; private set; }

    [SerializeField] private TMP_FontAsset dyslexiaFont;
    [SerializeField, Range(0.5f, 1f)] private float dyslexiaSizeMultiplier = 0.85f;

    private class OriginalStyle
    {
        public TMP_FontAsset font;
        public float size;
    }

    private readonly Dictionary<TMP_Text, OriginalStyle> originals = new Dictionary<TMP_Text, OriginalStyle>();

    private bool dyslexiaEnabled = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
        GameManager.OnServicesReady += LoadPreference;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GameManager.OnServicesReady -= LoadPreference;
    }

    private async void LoadPreference()
    {
        try
        {
            var loaded = await CloudSaveManager.Instance.LoadData(new HashSet<string> { "dyslexia_font_enabled" });
            dyslexiaEnabled = loaded.ContainsKey("dyslexia_font_enabled") &&
                               System.Convert.ToBoolean(loaded["dyslexia_font_enabled"]);

            Debug.Log($"DyslexiaFontManager: loaded preference = {dyslexiaEnabled}");
            ApplyFontToScene();
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyFontToScene();
    }

    private void ApplyFontToScene()
    {
        // drop entries for text objects destroyed by earlier scene loads
        var dead = new List<TMP_Text>();
        foreach (var key in originals.Keys)
        {
            if (key == null) dead.Add(key);
        }
        foreach (var key in dead) originals.Remove(key);

        TextMeshProUGUI[] allText = FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var text in allText)
        {
            // remember the authored font/size the first time we see this text
            if (!originals.ContainsKey(text))
            {
                originals[text] = new OriginalStyle { font = text.font, size = text.fontSize };
            }

            var original = originals[text];

            if (dyslexiaEnabled && dyslexiaFont != null)
            {
                text.font = dyslexiaFont;
                text.fontSize = original.size * dyslexiaSizeMultiplier;
            }
            else
            {
                text.font = original.font;
                text.fontSize = original.size;
            }
        }

        Debug.Log($"DyslexiaFontManager: applied {(dyslexiaEnabled ? "dyslexia" : "original")} font to {allText.Length} text objects");
    }

    public async Task SetDyslexiaFont(bool enabled)
    {
        dyslexiaEnabled = enabled;
        ApplyFontToScene();

        try
        {
            var data = new Dictionary<string, object> { { "dyslexia_font_enabled", enabled } };
            await CloudSaveManager.Instance.SaveData(data);
            Debug.Log($"DyslexiaFontManager: saved preference = {enabled}");
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public bool IsDyslexiaFontEnabled() => dyslexiaEnabled;
}