using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using UnityEngine;

public class CloudSaveManager : MonoBehaviour
{
    public static CloudSaveManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private async Task<bool> EnsureReady()
    {
        if (AuthenticationService.Instance.IsSignedIn)
            return true;

        Debug.LogWarning("CloudSaveManager: waiting for authentication...");

        for (int i = 0; i < 10; i++)
        {
            await Task.Delay(300);
            if (AuthenticationService.Instance.IsSignedIn)
                return true;
        }

        Debug.LogError("CloudSaveManager: UGS not ready — aborting.");
        return false;
    }

    /// <summary>
    /// Saves one or more key-value pairs to the player's Cloud Save data.
    /// Call with a Dictionary so you can save multiple fields at once (e.g. high score + unlocked items together).
    /// </summary>
    public async Task SaveData(Dictionary<string, object> data)
    {
        if (!await EnsureReady()) return;

        try
        {
            await CloudSaveService.Instance.Data.Player.SaveAsync(data);
            Debug.Log($"CloudSaveManager: saved {data.Count} key(s): {string.Join(", ", data.Keys)}");
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    /// <summary>
    /// Loads specific keys from Cloud Save. Pass the exact key names you want back.
    /// Returns an empty dictionary (not null) if nothing is found, so it's always safe to read from.
    /// </summary>
    public async Task<Dictionary<string, object>> LoadData(HashSet<string> keys)
    {
        if (!await EnsureReady()) return new Dictionary<string, object>();

        try
        {
            var results = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);

            var converted = new Dictionary<string, object>();
            foreach (var kvp in results)
            {
                converted[kvp.Key] = kvp.Value.Value.GetAs<object>();
            }

            Debug.Log($"CloudSaveManager: loaded {converted.Count} key(s)");
            return converted;
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
            return new Dictionary<string, object>();
        }
    }

    /// <summary>
    /// Loads ALL saved keys for this player, useful for the export-to-file feature later.
    /// </summary>
    public async Task<Dictionary<string, object>> LoadAllData()
    {
        if (!await EnsureReady()) return new Dictionary<string, object>();

        try
        {
            var results = await CloudSaveService.Instance.Data.Player.LoadAllAsync();

            var converted = new Dictionary<string, object>();
            foreach (var kvp in results)
            {
                converted[kvp.Key] = kvp.Value.Value.GetAs<object>();
            }

            Debug.Log($"CloudSaveManager: loaded all data, {converted.Count} key(s) total");
            return converted;
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
            return new Dictionary<string, object>();
        }
    }
}