using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static event System.Action OnServicesReady;
    public static event System.Action<int> OnHighScoreRestored; // NEW

    public int RestoredHighScore { get; private set; } = 0;

    async void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            Debug.Log("Services Initializing");
            await UnityServices.InitializeAsync();
        }

        await SignInAnonymouslyAsync();

        Debug.Log("Services ready — firing OnServicesReady");
        OnServicesReady?.Invoke();

        var loaded = await CloudSaveManager.Instance.LoadData(new HashSet<string> { "high_score" });
        if (loaded.ContainsKey("high_score"))
        {
            RestoredHighScore = System.Convert.ToInt32(loaded["high_score"]);
            Debug.Log($"Restored high score: {RestoredHighScore}");
            OnHighScoreRestored?.Invoke(RestoredHighScore); // NEW
        }
    }

    private async Task SignInAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign in anonymously succeeded!");
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }
}