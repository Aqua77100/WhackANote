using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiFade : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    private Canvas myCanvas; // store our own canvas component 
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float holdDurationBeforeFadeIn = 0.3f; // customisable hold delay before screen clears
    private static UiFade instance;

    private void Awake()
    {
        // so fade game object isn't destroyed during transitions to other scenes
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // grab the canvas component added to this object
            myCanvas = GetComponent<Canvas>();

            // run OnSceneLoaded every time a scene changes
            SceneManager.sceneLoaded += OnSceneLoaded;
        } 
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // clean up event listener if this object is ever destroyed
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // make sure canvas is turned on so we can see fade-in
        if (myCanvas != null)
        {
            myCanvas.enabled = true;
        }

        // start solid, then fade out
        // true: turn off canvas when finished, true: want to use hold delay
        StartCoroutine(ChangeAlphaCo(1f, 0f, true, true)); 
    }

    public void GoToScene(string sceneName)
    {
        // make sure canvas is turned on before fading-out
        if (myCanvas != null)
        {
            myCanvas.enabled = true;
            myCanvas.sortingOrder = 999;
        }

        // fading transition when loading scene
        StartCoroutine(FadeEffect(sceneName));
    }

    private IEnumerator FadeEffect(string sceneName)
    {
        // fade to black
        // false: keep canvas active because we are switching scenes, false: fade to black instantly
        yield return StartCoroutine(ChangeAlphaCo(0f, 1f, false, false));

        // load new scene
        yield return SceneManager.LoadSceneAsync(sceneName);
    }

    private IEnumerator ChangeAlphaCo(float startAlpha, float targetAlpha, bool disableCanvasOnFinish, bool useDelay)
    {
        if (useDelay && targetAlpha == 0f)
        {
            yield return new WaitForSecondsRealtime(holdDurationBeforeFadeIn);
        }

        float timePassed = 0;
        canvasGroup.alpha = startAlpha;

        while(timePassed < fadeDuration)
        {
            // take time (in sec) that it took from last frame to current one, add to timer 
            timePassed = timePassed + Time.unscaledDeltaTime;

            // take value between start and target value according to the time passed we have
            // (allows smooth transition of alpha during fade duration)
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timePassed / fadeDuration);

            yield return null; // waits until next frame

        }

        canvasGroup.alpha = targetAlpha; // snap the result

        if (disableCanvasOnFinish && targetAlpha == 0f && myCanvas != null)
        {
            // drop the sort order so its behind everything during gameplay
            myCanvas.sortingOrder = -1;

            // put canvas to sleep
            myCanvas.enabled = false;
        }
    }
}
