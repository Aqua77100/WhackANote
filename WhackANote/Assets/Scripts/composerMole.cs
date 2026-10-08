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
        if (Composer.Instance == null)
            return;

        if (!Composer.Instance.IsRecording)
            return;

        // record this mole
        Composer.Instance.RecordMole(moleIndex);

        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }

        // play press animation
        if(animator != null)
        {
            Debug.Log("animate");
            animator.SetTrigger("Pressed");
        } 

        Debug.Log("Composer mole pressed: " + moleIndex);
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
