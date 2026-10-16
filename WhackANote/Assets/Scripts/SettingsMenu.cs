using UnityEngine;
using DG.Tweening;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] RectTransform settingPanelRect;
    [SerializeField] float topPosY, middlePosY;
    [SerializeField] float tweenDuration;

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
        settingPanelRect.
    }

    void SettingPanelOutro()
    {

    }
}
