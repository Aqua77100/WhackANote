using UnityEngine;

public class SceneAudioSetup : MonoBehaviour
{
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // track volume is linked with the music slider volume from title screen
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            audioSource.volume = PlayerPrefs.GetFloat("musicVolume");
        }
    }
}
