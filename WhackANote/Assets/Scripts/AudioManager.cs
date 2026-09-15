using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio source:")]
    [SerializeField] AudioSource musicSource;

    [Header("Audio clip:")]
    public AudioClip titleBGM;
    public AudioClip circusTrack;

    public static AudioManager instance;

    private void Awake()
    {
        // if this instance has not been created before
        if (instance == null)
        {
            //let instance be this object
            instance = this;
            DontDestroyOnLoad(gameObject); // and dont destroy it
        } 
        else
        {
            Destroy(gameObject); // if it's been created before, destroy it
        }
    }
    private void Start()
    {
        musicSource.clip = titleBGM;
        musicSource.Play();
    }
}
