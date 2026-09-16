using UnityEngine;

public class ExportTester : MonoBehaviour
{
    async void Start()
    {
        await System.Threading.Tasks.Task.Delay(2000); // wait for services + sign-in
        string path = await DataExportManager.Instance.ExportPlayerData();
        if (path != null)
        {
            Debug.Log($"Exported successfully to: {path}");
        }
    }
}