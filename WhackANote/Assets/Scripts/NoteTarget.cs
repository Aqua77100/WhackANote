using UnityEngine;

public class NoteTarget : MonoBehaviour
{
    public void OnHit()
    {
        Debug.Log($"{gameObject.name} hit accurately!");

        // Hook in your existing hit logic here, e.g.:
        // ScoreManager.Instance.AddScore(100);
        // Play hit animation/particle
        // Destroy(gameObject) or deactivate
    }
}