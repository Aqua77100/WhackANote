
using UnityEngine;

public class composerMole : MonoBehaviour
{
    [Header("Composer Settings")]
    public int moleIndex;
    public AudioSource audioSource;
    public Animator animator;

    [Header("Instrument Sounds")]
    public AudioClip[] instrumentClips;

    private int selectedInstrument = 0;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    // Called when the global instrument changes
    public void SetInstrument(int index)
    {
        selectedInstrument = index;
    }

    private void OnMouseDown()
    {
        PressMole();
    }

    public void PressMole()
    {
        if (LevelSavePopup.IsPopupOpen)
            return;
            
        // Don't allow manual presses during preview
        if (Composer.Instance != null &&
            Composer.Instance.IsPreviewing)
        {
            return;
        }

        // Record only when recording
        if (Composer.Instance != null &&
            Composer.Instance.IsRecording)
        {
            Composer.Instance.RecordMole(moleIndex);
        }

        PlaySoundAndAnimation();
    }

    public void PlayPreview()
    {
        PlaySoundAndAnimation();
    }

    private void PlaySoundAndAnimation()
    {
        AudioClip clipToPlay = null;

        // Get the selected instrument's audio clip
        if (instrumentClips != null &&
            selectedInstrument >= 0 &&
            selectedInstrument < instrumentClips.Length)
        {
            clipToPlay = instrumentClips[selectedInstrument];
        }

        // Fall back to original audio clip
        if (clipToPlay == null && audioSource != null)
        {
            clipToPlay = audioSource.clip;
        }

        if (audioSource != null && clipToPlay != null)
        {
            audioSource.PlayOneShot(clipToPlay);
        }

        if (animator != null)
        {
            animator.SetTrigger("Pressed");
        }
    }
}
