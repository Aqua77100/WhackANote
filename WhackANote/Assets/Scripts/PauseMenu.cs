using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu; // This holds the restart, home, and continue button
    [SerializeField] private GameObject uiBlocker; // this is the dark screen that blocks the player's presses as well as allowing us to tell we're paused
    [SerializeField] private AudioSource Music;
    [SerializeField] private GameObject GameOverUI; // The  gameover (track cleared) panel, which has the retry (restart) and home button

    public TextMeshProUGUI countdownText;

    // booleans for the game states
    public static bool isPaused = false;
    public bool isEnded = false;
    private bool hasStartedPlaying = false;
    private bool gameStarted = false;

    private Coroutine countdownCoroutine; // Corouting needed for the countdown

    [Header("Tutorial Guard")]
    public bool isTutorialScene = false; // Keep this box in ticked in the Inspector for Tutorial scene!!!!!!

    private void Awake()
    {
        isPaused = false;
        isEnded = false;
        hasStartedPlaying = false;
        gameStarted = false;

        Time.timeScale = 0f;

        if (GameOverUI != null) GameOverUI.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (uiBlocker != null) uiBlocker.SetActive(true); // This will block the UI initially (with the countdown present) so user isnt thrown into game

        if (Music != null)
        {
            Music.Stop();
        }
    }

    private void Start()
    {
        countdownCoroutine = StartCoroutine(CountdownRoutine()); // on start, block UI and start countdown
    }

    private void Update()
    {
        // Do NOT trigger gameOver screen if this is the tutorial
        if (isTutorialScene || isEnded || !gameStarted || Music == null) return;

        if (Music.isPlaying)
        {
            hasStartedPlaying = true;
        }
        else if (hasStartedPlaying && Time.timeScale > 0)
        {
            gameOver(); // if the music has ended, then show game over screen
        }
    }

    public void Pause()
    {
        isPaused = true;

        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        if (pauseMenu != null) pauseMenu.SetActive(true); // set the pause menu to active
        if (uiBlocker != null) uiBlocker.SetActive(true); // block UI when paused (dark screen)

        Time.timeScale = 0f; // pauses time

        if (Music != null && Music.isPlaying)
        {
            Music.Pause(); // pause the music
        }
    }

    public void Continue() // click continue button then:
    {
        isPaused = false;

        if (pauseMenu != null) pauseMenu.SetActive(false); // remove pause menu display

        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine); // stop using the countdown if there is nothing (GameObject) in input bar 
        }

        countdownCoroutine = StartCoroutine(CountdownRoutine()); // start the countdown
    }

    IEnumerator CountdownRoutine() // this is the countdown coroutine
    {
        Time.timeScale = 0f; // make sure time is pased so game isn't going on
        if (uiBlocker != null) uiBlocker.SetActive(true); // Keep clicks blocked during countdown

        if (Music != null && Music.isPlaying)
        {
            Music.Pause(); // pause the music
        }

        if (countdownText != null) // checks if there is actually something in the text
        {
            countdownText.gameObject.SetActive(true); // set the visibility of text to true

            int countdownTime = 3; // 3 seconds

            while (countdownTime > 0) // while time is not zero:
            {
                countdownText.text = countdownTime.ToString(); // get the coundown time and make it a string
                yield return new WaitForSecondsRealtime(1f); // wait for 1 real second
                countdownTime--; // decrement time and set the next text to the countdownTime (T-1)
            }

            countdownText.text = "GO!"; // when reaching 0 (exiting the loop above), print "GO!"
            yield return new WaitForSecondsRealtime(1f); // wait for 1 second for player to read text

            countdownText.gameObject.SetActive(false); // now set the visibility to false
        }


        Time.timeScale = 1f; // play normal time speed
        if (uiBlocker != null) uiBlocker.SetActive(false); // Unblock interactions after countdown

        if (Music != null)
        {
            if (hasStartedPlaying)  // if music has already been playing:
            {
                Music.UnPause(); //unpause
            }
            else // if it hasnt played yet (e.g. you paused when just starting the game):
            {
                Music.Play(); // play
                hasStartedPlaying = true; // set boolean to true so that next time, it will unpause where you left off
            }
        }

        gameStarted = true; // game has started
        countdownCoroutine = null;
    }

    public void Restart()
    {
        CleanupBeforeSceneChange(); // Reset to initial states (see method below)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // get the scene and load it
    }

    public void Home()
    {
        CleanupBeforeSceneChange(); // reset to initial states so when played again, it is alright
        SceneManager.LoadScene("Menu"); // load menu screen -- CHANGE THIS TO 'TITLE' IF WANTING TO RENAME
    }

    private void CleanupBeforeSceneChange() // reset the game states
    {
        isEnded = true;
        hasStartedPlaying = false;
        gameStarted = false;

        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }

        if (GameOverUI != null) GameOverUI.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(false);

        Time.timeScale = 1f;
    }

    public void gameOver()
    {
        if (!isEnded) // check the boolean
        {
            isEnded = true; // set to true
            Time.timeScale = 0f;
            if (GameOverUI != null) GameOverUI.SetActive(true); // show gameover UI and the UI blocker 
            if (uiBlocker != null) uiBlocker.SetActive(true);
        }
    }

    // This is for the last test case about mobile interruptions--not sure how to test these, got these from google
    private void OnApplicationFocus(bool hasFocus)
    {
        // If the app loses focus (phone call, home button, app switcher) and game isn't already ended
        if (!hasFocus && !isEnded && gameStarted)
        {
            Pause();
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        // Alternative check required by some Android devices when suspended
        if (pauseStatus && !isEnded && gameStarted)
        {
            Pause();
        }
    }
}