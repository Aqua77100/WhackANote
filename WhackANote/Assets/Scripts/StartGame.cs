using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public string LevelName;
    public static string CurrentTrackId; // NEW — remembers which track was launched, survives the scene load

    public void LoadLevel()
    {
        CurrentTrackId = LevelName; // NEW
        SceneManager.LoadScene(LevelName);
    }
}