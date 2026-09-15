using UnityEngine;
using UnityEngine.SceneManagement;

public class TrackSelectionBack : MonoBehaviour
{
    public string title;

    public void LoadTitle()
    {
        SceneManager.LoadScene("Menu");
    }
}
