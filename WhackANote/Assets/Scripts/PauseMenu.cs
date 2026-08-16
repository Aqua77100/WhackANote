using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private AudioSource Music;
    [SerializeField] private GameObject GameOverUI;

    public bool isEnded = false;
    private bool hasStartedPlaying = false;

    private void Update()
    {
        if (isEnded || Music == null) return;

        // Detect when the music starts playing
        if (Music.isPlaying)
        {
            hasStartedPlaying = true;
        }
        // Once it has played and now stopped (and game isn't paused), trigger Game Over
        else if (hasStartedPlaying && Time.timeScale > 0)
        {
            gameOver();
        }
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;

        if (Music != null)
        {
            Music.Pause();
        }
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Home()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }

    public void Continue()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;

        if (Music != null)
        {
            Music.UnPause();
        }
    }

    public void gameOver()
    {
        if (!isEnded)
        {
            isEnded = true;
            if (GameOverUI != null)
            {
                GameOverUI.SetActive(true);
            }
        }
    }
}