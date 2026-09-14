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

    void Start()
    {
        StartAnonymousSignIn();
    }

    // Initializes unity gaming services (required before authentication) if it hasn't been initialized yet.
    async void Awake()
    {
        if(UnityServices.State == ServicesInitializationState.Uninitialized) 
        {
            Debug.Log("Services Initializing");
            // waits for Unity's backend services to finish setting up.
            await UnityServices.InitializeAsync();

        }

    // public entry point - kicks off the async sign-in process.
    public async void StartAnonymousSignIn()
    {
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

    // signs the user into unity authentication anonymously
    // generates a persistent anonymous player ID tied to the device/install
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