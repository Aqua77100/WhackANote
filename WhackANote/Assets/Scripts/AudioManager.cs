using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio source:")]
    [SerializeField] AudioSource musicSource;

    [Header("Audio clip:")]
    public AudioClip background;

    private void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }
}
