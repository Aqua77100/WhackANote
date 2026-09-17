using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialPopupDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statusValueText; // the "Incomplete"/"Completed" text object

    private async void OnEnable()
    {
        try
        {
            var loaded = await CloudSaveManager.Instance.LoadData(new HashSet<string> { "tutorial_completed" });

            bool isCompleted = loaded.ContainsKey("tutorial_completed") &&
                                System.Convert.ToBoolean(loaded["tutorial_completed"]);

            if (statusValueText != null)
            {
                statusValueText.text = isCompleted ? "Completed" : "Incomplete";
            }

            Debug.Log($"TutorialPopupDisplay: tutorial_completed = {isCompleted}");
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }
    }
}