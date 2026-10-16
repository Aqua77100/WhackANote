using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public string LevelName;
    public static string CurrentTrackId; // NEW — remembers which track was launched, survives the scene load

    public void LoadLevel()
    {
        CurrentTrackId = LevelName; // NEW

        // find UiFade and tell it to switch scenes
        UiFade fader = Object.FindAnyObjectByType<UiFade>();
        if (fader != null)
        {
            fader.GoToScene(LevelName);
        }
        else
        {
            SceneManager.LoadScene(LevelName); // load normally
        }
    }

    public void LoadLeaderboard()
    {
        CurrentTrackId = LevelName;

        // find UiFade and tell it to switch scenes
        UiFade fader = Object.FindAnyObjectByType<UiFade>();
        if (fader != null)
        {
            fader.GoToScene("Leaderboard");
        }
        else
        {
            SceneManager.LoadScene("Leaderboard"); // load normally
        }
    }
}