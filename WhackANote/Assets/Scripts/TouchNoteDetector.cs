using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class TouchNoteDetector : MonoBehaviour
{
    [SerializeField] private Camera gameCamera;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    private void Update()
    {
        if (gameCamera == null) gameCamera = Camera.main;

        // Handle touch input (mobile)
        foreach (var touch in Touch.activeTouches)
        {
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                TryHit(touch.screenPosition);
            }
        }

        // Handle mouse input (Editor testing)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryHit(Mouse.current.position.ReadValue());
        }
    }

    private void TryHit(Vector2 screenPosition)
    {
        Vector2 worldPoint = gameCamera.ScreenToWorldPoint(screenPosition);
        Collider2D hitCollider = Physics2D.OverlapPoint(worldPoint);

        if (hitCollider != null)
        {
            NoteTarget note = hitCollider.GetComponent<NoteTarget>();
            if (note != null)
            {
                note.OnHit();
            }
        }
    }
}