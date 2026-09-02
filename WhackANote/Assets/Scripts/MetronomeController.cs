using UnityEngine;

public class MetronomeController : MonoBehaviour
{
    public Animator animator;
    public float bpm = 100f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (animator = null)
        {
            animator = GetComponent<Animator>();
        }

        animator.speed = bpm / 60f;
    }
}
