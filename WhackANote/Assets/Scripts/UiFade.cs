using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiFade : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 1f;
    private static UiFade instance;

    private void Start()
    {
        StartCoroutine(ChangeAlphaCo(1, 0)); // automatically fade in when the scene starts
    }

    private void Awake()
    {
        // so fade game object isn't destroyed during transitions to other scenes
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

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
        // start solid, then fade out
        StartCoroutine(ChangeAlphaCo(1, 0));
    }

    public void GoToScene(string sceneName)
    {
        // fading transition when loading scene
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
