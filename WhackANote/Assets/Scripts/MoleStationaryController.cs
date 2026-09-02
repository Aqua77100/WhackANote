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
    public TextMeshProUGUI HitType;

    [Header("Hit Type Colours")] // These RGB codes actually dont really work, so I manually added them on the moles, hence the header and public types
    public Color perfectColour = new Color32(255, 238, 129, 255);
    public Color greatColour = new Color32(130, 255, 229, 255);
    public Color goodColour = new Color32(244, 153, 252, 255);
    public Color missColour = new Color32(142, 88, 85, 255);

    [Header("Animation Settings")]
    public SpriteRenderer spriteRenderer;
    [Tooltip("Assign 12 sprites in sequence (Frames 0 to 11)")]
    public Sprite[] moleSprites;
    public float frameRate = 0.05f;

    private bool isClickable = false;
    private bool wasTapped = false;
    private Coroutine activeRoutine;
    private Coroutine textFadeRoutine; // New corouting for the text fading
    private Camera mainCamera;


    [Header("Scoring Values")] // Again, public types to change for the different score types
    public int perfectScore = 50;
    public int greatScore = 30;
    public int goodScore = 15;

    private int currentHitFrame = 7; // To keep track of what frame of the mole array we're on

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
            yield return new WaitForSeconds(frameRate * 0.5f);
        }

        // 2. Interactive Window (Wait duration, cancel immediately if tapped)
        isClickable = true;
        float thirdDuration = interactiveDuration / 3f; // Having 3 optionsfor hit types means we need 3 durations, each 1/3 of the total active time

        // Frame 7 (Perfect! hit window)
        currentHitFrame = 7;
        spriteRenderer.sprite = moleSprites[6]; // Frame 7 is index 6
        float timer = 0f;

        while (timer < thirdDuration && !wasTapped)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // Frame 8 (Great! hit window)
        if (!wasTapped)
        {
            currentHitFrame = 8;
            spriteRenderer.sprite = moleSprites[7]; // Frame 8 is index 7
            timer = 0f;
            while (timer < thirdDuration && !wasTapped)
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }

        // Frame 9 (Good! hit window)
        if (!wasTapped)
        {
            currentHitFrame = 9;
            spriteRenderer.sprite = moleSprites[8]; // Frame 9 is index 8
            timer = 0f;
            while (timer < thirdDuration && !wasTapped)
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }

        isClickable = false;

        if (!wasTapped) // If you miss the tap, display MISS
        {
            ShowHitText("MISS", missColour);
        }

        // 3. Pop Down (Frames 9 to 13)
        for (int i = 9; i < moleSprites.Length && i < 14; i++)
        {
            spriteRenderer.sprite = moleSprites[i];
            yield return new WaitForSeconds(frameRate * 0.3f);
        }
    }

    // Retained for desktop testing/editor mouse clicks
    private void OnMouseDown()
    {
        // If the click is over a UI button or UI element, STOP processing world input!
        // if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        // {
        //     return;
        // }

        ProcessTap();
    }

    // Interface implementation for EventSystem/UI touch detection
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            if (eventData.pointerCurrentRaycast.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                return;
            }
        }

        ProcessTap();
        // if (Input.touchCount > 0 && isClickable && !wasTapped)
        // {
        //     Touch touch = Input.GetTouch(0);

        //     // Check if touch ID is over a UI element
        //     if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        //     {
        //         return; // Ignore world tap if touching UI
        //     }

        //     // Block UI pointer events over moles during pause
        //     if (eventData.pointerCurrentRaycast.gameObject != null)
        //     {
        //         // If the tap hit a UI element (like Dark Panel or CountdownText) instead of world space, ignore
        //         if (eventData.pointerCurrentRaycast.gameObject.layer == LayerMask.NameToLayer("UI"))
        //         {
        //             return;
        //         }
        //     }

        //     ProcessTap();
        // }

    }

    // Centralized method to process hit logic safely once
    private void ProcessTap()
    {
        // Block processing if paused or frozen
        if (PauseMenu.isPaused || Time.timeScale == 0f) return;

        if (isClickable && !wasTapped)
        {
            wasTapped = true;
            
            // Notify tutorial manager if running
            TutorialManager tutorial = Object.FindAnyObjectByType<TutorialManager>();
            if (tutorial != null)
            {
                // Find index of this mole in the array or pass its reference
                tutorial.OnMoleTapped(System.Array.IndexOf(tutorial.moles, this));
            }

            // initialise the points, and base for the text
            int points = 0;
            string feedbackText = "";
            Color feedbackColour = Color.white;

            switch (currentHitFrame)
            {
                case 7: // This uses the index value above to find which hit the player did & sets the corresponding text string and colour
                    points = perfectScore; // Frame 7 = Perfect
                    feedbackText = "PERFECT!";
                    feedbackColour = perfectColour;
                    break;
                case 8:
                    points = greatScore;   // Frame 8 = Great
                    feedbackText = "GREAT!";
                    feedbackColour = greatColour;
                    break;
                case 9:
                    points = goodScore;    // Frame 9 = Good
                    feedbackText = "GOOD!";
                    feedbackColour = goodColour;
                    break;
            }
            ShowHitText(feedbackText, feedbackColour); // Display using the helper method below using the resulting string and colour

            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(points);
            }

            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.PlayOneShot(audioSource.clip);
            }
        }
    }

    private void ShowHitText(string text, Color colour)
    {
        if (HitType == null)
        {
            Debug.LogWarning("HitType TextMeshProUGUI is not assigned in Inspector on " + gameObject.name); // You didn't assign the text
            return;
        }

        // Don't spawn text if paused
        if (HitType == null || PauseMenu.isPaused) return;

        // take the input for the string for the case and the colour
        HitType.text = text;
        HitType.color = new Color(colour.r, colour.g, colour.b, 1f);
        HitType.gameObject.SetActive(true); // Make text visible

        if (textFadeRoutine != null)
        {
            StopCoroutine(textFadeRoutine);
        }
        textFadeRoutine = StartCoroutine(HideTextAfterDelay(0.2f, 0.2f)); // Control the speed of how the text disappears using floats
    }

    private IEnumerator HideTextAfterDelay(float holdDuration, float fadeDuration)
    {
        // This method was from Google, using the Mathf.Lerp
        yield return new WaitForSeconds(holdDuration); // Wait for 0.2f
        Color startColour = HitType.color; // Get the texts current colour
        float elapsed = 0f; // timer variable

        while (elapsed < fadeDuration) //While still within the window for fading:
        {
            elapsed += Time.deltaTime; // Use deltaTime to get real amount of time
            // Create a new number for transparency over the duration (gets lesser over time)
            float newAlpha = Mathf.Lerp(startColour.a, 0f, elapsed / fadeDuration);

            // Preserve RGB values and lower only the alpha (transparency) channel
            HitType.color = new Color(startColour.r, startColour.g, startColour.b, newAlpha);
            yield return null;
        }

        // Ensure alpha (transparency) is fully zero (invisible) before disabling
        HitType.color = new Color(startColour.r, startColour.g, startColour.b, 0f);
        HitType.gameObject.SetActive(false);
    }

    // When pause menu = active --> listen (subscribe) to the pause and resume events
    private void OnEnable()
    {
        PauseMenu.OnGamePaused += HideHitTextOnPause;
    }

    // When pause menu = inactive --> unsubscribes from the same events
    private void OnDisable()
    {
        PauseMenu.OnGamePaused -= HideHitTextOnPause;
    }

    // Runs automatically when game pauses --> hides the HitType game object if visible
    private void HideHitTextOnPause()
    {
        if (textFadeRoutine != null)
        {
            StopCoroutine(textFadeRoutine);
            textFadeRoutine = null;
        }

        if (HitType != null)
        {
            HitType.gameObject.SetActive(false);
        }
        // if (HitType != null && HitType.gameObject.activeSelf)
        // {
        //     HitType.gameObject.SetActive(false);
        // }
    }


    // Runs automatically when game resumes --> reveals HitType object ONLY if textFadeRoutine was actively running before pausing
    // private void HandleResume()
    // {
    //     // If the fade routine was active when paused --> re-enable the text so it can complete fading
    //     if (textFadeRoutine != null && HitType != null)
    //     {
    //         HitType.gameObject.SetActive(true);
    //     }
    // }
}