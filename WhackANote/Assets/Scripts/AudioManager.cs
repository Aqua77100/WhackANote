using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio source:")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("Audio clip:")]
    public AudioClip background;
    public AudioClip moleAS3;
    public AudioClip moleB3;
    public AudioClip moleC4;
    public AudioClip moleCS4;
    public AudioClip moleD4;
    public AudioClip moleE4;
    public AudioClip moleFS4;
    public AudioClip moleGS3;


    private void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }
}
