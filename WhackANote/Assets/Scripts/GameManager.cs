using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;

public class GameManager : MonoBehaviour
{

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
    }

    // public entry point - kicks off the async sign-in process.
    public async void StartAnonymousSignIn()
    {
        await SignInAnonymouslyAsync();
    }

    // signs the user into unity authentication anonymously
    // generates a persistent anonymous player ID tied to the device/install
    private async Task SignInAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign in anonymously succeeded!");

            // Shows how to get the playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }
}
