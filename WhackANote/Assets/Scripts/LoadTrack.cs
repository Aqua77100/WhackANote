using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadTrack : MonoBehaviour
{
    public string trackName;

    public void OpenTrack()
    {
        SceneManager.LoadScene(trackName);
    }
}
