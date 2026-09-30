using System.Collections.Generic;
using UnityEngine;

public class DebugTools : MonoBehaviour
{
    [SerializeField] private string trackIdToReset = "Circus";

    [ContextMenu("Reset High Score For Track")]
    private async void ResetHighScore()
    {
        string key = $"highscore_{trackIdToReset}";
        await CloudSaveManager.Instance.SaveData(new Dictionary<string, object> { { key, 0 } });
        Debug.Log($"Reset {key} to 0 in Cloud Save");
    }

    [ContextMenu("Reset Tutorial Completion")]
    private async void ResetTutorialCompletion()
    {
        await CloudSaveManager.Instance.SaveData(new Dictionary<string, object> { { "tutorial_completed", false } });
        Debug.Log("Reset tutorial_completed to false in Cloud Save");
    }
}