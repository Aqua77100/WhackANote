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
        // Ignore inputs if the game is paused or counting down
        if (PauseMenu.isPaused || Time.timeScale == 0f) return;

        // Explicitly handle Mobile Touch input
        if (Input.touchCount > 0 && isClickable && !wasTapped)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    Vector2 touchWorldPos = mainCamera.ScreenToWorldPoint(touch.position);
                    
                    // Raycast specifically for 2D colliders
                    RaycastHit2D hit = Physics2D.Raycast(touchWorldPos, Vector2.zero);

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
        // Block UI pointer events over moles during pause
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            // If the tap hit a UI element (like Dark Panel or CountdownText) instead of world space, ignore
            if (eventData.pointerCurrentRaycast.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                return;
            }
        }
        
        ProcessTap();
    }

    // Centralized method to process hit logic safely once
    private void ProcessTap()
    {
        // Block processing if paused or frozen
        if (PauseMenu.isPaused || Time.timeScale == 0f) return;

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