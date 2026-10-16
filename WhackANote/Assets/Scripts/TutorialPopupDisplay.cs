using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class TutorialPopupDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statusValueText; // the "Incomplete"/"Completed" text object
    [SerializeField] RectTransform tutorialPanelRect;
    [SerializeField] float topPosY, middlePosY;
    [SerializeField] float tweenDuration;
    [SerializeField] CanvasGroup canvasGroup; // dark panel

    private async void OnEnable()
    {
        SettingPanelIntro();
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

    public void Close()
    {
        SettingPanelOutro();
    }

    void SettingPanelIntro()
    {
        canvasGroup.DOFade(1, tweenDuration);
        tutorialPanelRect.DOAnchorPosY(middlePosY, tweenDuration);
    }

    void SettingPanelOutro()
    {
        canvasGroup.DOFade(0, tweenDuration);
        tutorialPanelRect.DOAnchorPosY(topPosY, tweenDuration)
            .OnComplete(() => gameObject.SetActive(false));
    }
}