using UnityEngine;
using DG.Tweening;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] RectTransform settingPanelRect;
    [SerializeField] float topPosY, middlePosY;
    [SerializeField] float tweenDuration;
    [SerializeField] CanvasGroup canvasGroup; // dark panel

    private void OnEnable()
    {
        SettingPanelIntro();
    }

    public void Close()
    {
        SettingPanelOutro();
    }

    void SettingPanelIntro()
    {
        canvasGroup.DOFade(1, tweenDuration);
        settingPanelRect.DOAnchorPosY(middlePosY, tweenDuration);
    }

    void SettingPanelOutro()
    {
        canvasGroup.DOFade(0, tweenDuration);
        settingPanelRect.DOAnchorPosY(topPosY, tweenDuration)
            .OnComplete(() => gameObject.SetActive(false));
    }
}
