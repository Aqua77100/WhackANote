
using UnityEngine;

public class composerMole : MonoBehaviour
{
    [Header("Composer Settings")]
    public int moleIndex;
    public AudioSource audioSource;
    public Animator animator;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    private void OnMouseDown()
    {
        PressMole();
    }

    
    public void PressMole()
    {
        // Prevent manual clicking during preview
        if (Composer.Instance != null &&
            Composer.Instance.IsPreviewing)
        {
            return;
        }

        // Only record the note if recording is active
        if (Composer.Instance != null &&
            Composer.Instance.IsRecording)
        {
            Composer.Instance.RecordMole(moleIndex);
        }

        // Play the mole's sound
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }

        // Play the press animation
        if (animator != null)
        {
            animator.SetTrigger("Pressed");
        }
    }


    public void PlayPreview()
    {
        // Play the mole's note
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }

        // Play the mole animation
        if (animator != null)
        {
            animator.SetTrigger("Pressed");
        }
    }
}
