using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class DataExportManager : MonoBehaviour
{
    public static DataExportManager Instance { get; private set; }

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

    /// <summary>
    /// Loads all Cloud Save data for the player and writes it to a JSON file
    /// at Application.persistentDataPath. Returns the full file path on success, or null on failure.
    /// </summary>
    public async Task<string> ExportPlayerData()
    {
        var allData = await CloudSaveManager.Instance.LoadAllData();

        if (allData.Count == 0)
        {
            Debug.LogWarning("DataExportManager: no data found to export.");
            return null;
        }

        try
        {
            var wrapper = new SerializableDataWrapper();
            foreach (var kvp in allData)
            {
                wrapper.entries.Add(new DataEntry { key = kvp.Key, value = kvp.Value.ToString() });
            }

            string json = JsonUtility.ToJson(wrapper, true);

            string fileName = $"WhackANote_PlayerData_{System.DateTime.Now:yyyyMMdd_HHmmss}.json";
            string filePath = Path.Combine(Application.persistentDataPath, fileName);

            File.WriteAllText(filePath, json);

            Debug.Log($"DataExportManager: exported data to {filePath}");
            return filePath;
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
            return null;
        }
    }
}

[System.Serializable]
public class SerializableDataWrapper
{
    public List<DataEntry> entries = new List<DataEntry>();
}

[System.Serializable]
public class DataEntry
{
    public string key;
    public string value;
}