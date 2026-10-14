using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiFade : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 1f;

    private void Start()
    {
        StartCoroutine(ChangeAlphaCo(1, 0)); // automatically fade in when the scene starts
    }

    public void GoToScene(string sceneName)
    {
        StartCoroutine(FadeEffect(sceneName));
    }

    private IEnumerator FadeEffect(string sceneName)
    {
        // fade to black
        yield return StartCoroutine(ChangeAlphaCo(0, 1));

        // load new scene
        yield return SceneManager.LoadSceneAsync(sceneName);
    }

    private IEnumerator ChangeAlphaCo(float startAlpha, float targetAlpha)
    {
        float timePassed = 0;
        canvasGroup.alpha = startAlpha;

        while(timePassed < fadeDuration)
        {
            // take time (in sec) that it took from last frame to current one, add to timer 
            timePassed = timePassed + Time.deltaTime;

            // take value between start and target value according to the time passed we have
            // (allows smooth transition of alpha during fade duration)
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timePassed / fadeDuration);

            yield return null; // waits until next frame

        }

        canvasGroup.alpha = targetAlpha; // snap the result
    }
}
