using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MetronomeController : MonoBehaviour
{
    public Animator animator;
    public float bpm = 100f;

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        animator.speed = bpm / 60f;
    }
}
