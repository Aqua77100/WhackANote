using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;

public class UsernameSettings : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TextMeshProUGUI currentNameText;
    [SerializeField] private TextMeshProUGUI statusText;

    // Runs each time the Settings panel is opened
    private async void OnEnable()
    {
        statusText.text = "";
        nameInput.characterLimit = 20; // my choice; Unity allows up to 50

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            currentNameText.text = "Current name: ...";
            return;
        }

        try
        {
            string name = await AuthenticationService.Instance.GetPlayerNameAsync();
            currentNameText.text = $"Current name: {name}";
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    // Wire the Save button's OnClick to this
    public async void OnSaveClicked()
    {
        string requested = nameInput.text.Trim();

        if (requested.Length == 0)
        {
            statusText.text = "Enter a name first.";
            return;
        }

        if (Regex.IsMatch(requested, @"\s"))
        {
            statusText.text = "Names can't contain spaces.";
            return;
        }

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            statusText.text = "Still connecting, try again in a moment.";
            return;
        }

        try
        {
            string result = await AuthenticationService.Instance.UpdatePlayerNameAsync(requested);
            currentNameText.text = $"Current name: {result}";
            statusText.text = "Name saved!";
            nameInput.text = "";
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
            statusText.text = "That name isn't allowed.";
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
            statusText.text = "Couldn't save. Check your connection.";
        }
    }
}