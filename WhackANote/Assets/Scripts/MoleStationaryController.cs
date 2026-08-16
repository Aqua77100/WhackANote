using System.Collections;
using UnityEngine;
using TMPro;
// Added to support EventTrigger / Pointer events for UI or Touch Raycasting if needed
using UnityEngine.EventSystems; 

public class MoleStationaryController : MonoBehaviour, IPointerDownHandler
{
    [Header("UI & Audio")]
    public AudioSource audioSource;
    public AudioClip moleNote;

    [Header("Animation Settings")]
    public SpriteRenderer spriteRenderer;
    [Tooltip("Assign 12 sprites in sequence (Frames 0 to 11)")]
    public Sprite[] moleSprites; 
    public float frameRate = 0.05f;

    private bool isClickable = false;
    private bool wasTapped = false;
    private Coroutine activeRoutine;
    private Camera mainCamera;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource != null && moleNote != null)
        {
            audioSource.clip = moleNote;
        }

        // Cache main camera for touch raycasting
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // Explicitly handle Mobile Touch input to ensure Android taps register reliably
        if (Input.touchCount > 0 && isClickable && !wasTapped)
        {
            foreach (Touch touch in Input.touches)
            {
                // Trigger tap as soon as the finger touches the screen
                if (touch.phase == TouchPhase.Began)
                {
                    // Convert touch screen position to 2D world coordinates
                    Vector2 touchWorldPos = mainCamera.ScreenToWorldPoint(touch.position);
                    
                    // Raycast specifically for 2D colliders
                    RaycastHit2D hit = Physics2D.Raycast(touchWorldPos, Vector2.zero);

                    // Check if the touch raycast hit THIS mole object
                    if (hit.collider != null && hit.collider.gameObject == gameObject)
                    {
                        ProcessTap();
                        break;
                    }
                }
            }
        }
    }

    // Called externally by RhythmSequencer
    public void PopUp(float interactiveDuration)
    {
        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
        }
        activeRoutine = StartCoroutine(MoleRoutine(interactiveDuration));
    }

    private IEnumerator MoleRoutine(float interactiveDuration)
    {
        if (moleSprites == null || moleSprites.Length == 0)
        {
            Debug.LogError("Mole Sprites array is empty!");
            yield break;
        }

        wasTapped = false;

        // 1. Pop Up (Frames 0 to 6)
        for (int i = 0; i <= 6 && i < moleSprites.Length; i++)
        {
            spriteRenderer.sprite = moleSprites[i];
            yield return new WaitForSeconds(frameRate);
        }

        // 2. Interactive Window (Wait duration, cancel immediately if tapped)
        isClickable = true;
        float timer = 0f;

        while (timer < interactiveDuration && !wasTapped)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        isClickable = false;

        // 3. Pop Down (Frames 7 to 11)
        for (int i = 7; i < moleSprites.Length && i < 12; i++)
        {
            spriteRenderer.sprite = moleSprites[i];
            yield return new WaitForSeconds(frameRate);
        }
    }

    // Retained for desktop testing/editor mouse clicks
    private void OnMouseDown()
    {
        ProcessTap();
    }

    // Interface implementation for EventSystem/UI touch detection
    public void OnPointerDown(PointerEventData eventData)
    {
        ProcessTap();
    }

    // Centralized method to process hit logic safely once
    private void ProcessTap()
    {
        if (isClickable && !wasTapped)
        {
            wasTapped = true;

            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(25);
            }

            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.PlayOneShot(audioSource.clip);
            }
        }
    }
}