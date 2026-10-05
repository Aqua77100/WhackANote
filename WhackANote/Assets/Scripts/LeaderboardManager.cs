using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance { get; private set; }

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

        Debug.LogWarning("Waiting for authentication...");

        for (int i = 0; i < 10; i++)
        {
            await Task.Delay(300);
            if (AuthenticationService.Instance.IsSignedIn)
                return true;
        }

        Debug.LogError("UGS not ready — aborting leaderboard call.");
        return false;
    }

    public async Task SubmitScore(int score, string trackId)
    {
        if (!await EnsureReady()) return;

        string leaderboardId = $"High_Scores_{trackId}";

        try
        {
            var result = await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardId, score);
            Debug.Log($"Score submitted to {leaderboardId}: {result.Score} (Rank {result.Rank + 1})");
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    public async Task<List<LeaderboardEntry>> GetTopScores(string trackId, int limit = 10)
    {
        if (!await EnsureReady()) return null;

        string leaderboardId = $"High_Scores_{trackId}";

        try
        {
            var options = new GetScoresOptions { Limit = limit };
            var response = await LeaderboardsService.Instance.GetScoresAsync(leaderboardId, options);

            foreach (var entry in response.Results)
            {
                Debug.Log($"#{entry.Rank + 1} — {entry.PlayerName ?? entry.PlayerId} — {entry.Score}");
            }

            return response.Results;
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
            return null;
        }
    }

    public async Task<LeaderboardEntry> GetPlayerScore(string trackId)
    {
        if (!await EnsureReady()) return null;

        string leaderboardId = $"High_Scores_{trackId}";

        try
        {
            return await LeaderboardsService.Instance.GetPlayerScoreAsync(leaderboardId);
        }
        catch (RequestFailedException ex)
        {
            // Throws if the player has no score on this leaderboard yet; that's expected
            Debug.LogWarning($"Player has no score yet on {leaderboardId}: {ex.Message}");
            return null;
        }
    }
}