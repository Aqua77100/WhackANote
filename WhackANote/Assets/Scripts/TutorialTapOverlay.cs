using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialTapOverlay : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TutorialManager tutorialManager;

    // IPointerClickHandler automatically respects Canvas Depth, Raycasts, and UI Blockers
    public void OnPointerClick(PointerEventData eventData)
    {
        // Ignore taps if the game is paused
        if (PauseMenu.isPaused) return;

        if (tutorialManager != null)
        {
            tutorialManager.OnTapOverlayClicked();
        }
    }
}