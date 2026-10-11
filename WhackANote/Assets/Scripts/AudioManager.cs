using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("Audio source:")]
    [SerializeField] AudioSource musicSource;

    [Header("Audio clip:")]
    public AudioClip titleBGM;
    public AudioClip circusTrack;
    public AudioClip tutorialTrack;

    public static AudioManager instance;

    private void Awake()
    {
        // so title BGM carries on to track selection
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

    //pause the titleBGM when on one of the tracks
    private void Update()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name == "tutorial" || currentScene.name == "Circus" || currentScene.name == "MMC")
        {
            musicSource.Pause();
        }
        else // once back on the title or track selection, play the title BGM again
        {
            if (!musicSource.isPlaying)
            {
                musicSource.clip = titleBGM;
                musicSource.Play();
            }
        }
    }
}
