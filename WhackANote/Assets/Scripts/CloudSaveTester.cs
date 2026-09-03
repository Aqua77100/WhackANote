using System.Collections.Generic;
using UnityEngine;

public class CloudSaveTester : MonoBehaviour
{
    async void Start()
    {
        await System.Threading.Tasks.Task.Delay(1500); // crude wait for sign-in, testing only

        var data = new Dictionary<string, object>
        {
            { "high_score", 999 },
            { "unlocked_items", new List<string> { "hat_red", "mole_gold" } }
        };

        await CloudSaveManager.Instance.SaveData(data);

        var loaded = await CloudSaveManager.Instance.LoadData(new HashSet<string> { "high_score", "unlocked_items" });
        foreach (var kvp in loaded)
        {
            Debug.Log($"{kvp.Key}: {kvp.Value}");
        }
    }
}