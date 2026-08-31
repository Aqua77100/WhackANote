using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio source:")]
    [SerializeField] AudioSource musicSource;

    [Header("Audio clip:")]
    public AudioClip titleBGM;
    public AudioClip circusTrack;

    private void Start()
    {
        musicSource.clip = titleBGM;
        musicSource.Play();
    }
}
