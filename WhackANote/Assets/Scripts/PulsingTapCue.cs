using UnityEngine;

public class PulsingTapCue : MonoBehaviour
{
    // THis will be for the png that I made (set of circles)
    public float pulseSpeed = 4f; // How fast we want it to pulse
    public float minScale = 0.85f; // Smallest size
    public float maxScale = 1.15f; // biggest size

    private void Update()
    {
        // Using that lerp thing again for the pulse animation
        // switch to the different sizes on the timing of the pulse speed
        float scale = Mathf.Lerp(minScale, maxScale, (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f); 
        transform.localScale = new Vector3(scale, scale, 1f); 
    }
}