using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Toggle dyslexiaToggle; // NEW

    // if player prefs for music volume has already been set, load it. if not, set volume as usual
    private void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMusicVolume();
        }
    }

    // NEW — runs every time this GameObject (the Settings panel) is activated
    private void OnEnable()
    {
        if (dyslexiaToggle != null && DyslexiaFontManager.Instance != null)
        {
            dyslexiaToggle.SetIsOnWithoutNotify(DyslexiaFontManager.Instance.IsDyslexiaFontEnabled());
        }
    }

    // set volume and volume slider matches with the music audio mixer in min/max value
    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        myMixer.SetFloat("music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    // keep the volume setting from the player that was previously set before
    private void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");

        SetMusicVolume();
    }

    // NEW — called by the dyslexia toggle's OnValueChanged event
    public async void OnDyslexiaToggleChanged(bool isOn)
    {
        await DyslexiaFontManager.Instance.SetDyslexiaFont(isOn);
    }
}