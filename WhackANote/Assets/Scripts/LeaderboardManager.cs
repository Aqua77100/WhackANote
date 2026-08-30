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

    private const string LeaderboardId = "High_Scores";

    private bool authReady = false;

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

    private void Start()
    {
        // SAFE: Unity Services are already initializing by now
        AuthenticationService.Instance.SignedIn += () =>
        {
            authReady = true;
            Debug.Log($"Signed in automatically. PlayerID: {AuthenticationService.Instance.PlayerId}");
        };
    }

    private async Task<bool> EnsureReady()
    {
        if (!authReady)
        {
            Debug.LogWarning("Waiting for authentication...");
            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(200);
                if (authReady) break;
            }
        }

        if (!authReady)
        {
            Debug.LogError("UGS not ready — aborting leaderboard call.");
            return false;
        }

        return true;
    }

    public async Task SubmitScore(int score)
    {
        if (!await EnsureReady()) return;

        try
        {
            var result = await LeaderboardsService.Instance.AddPlayerScoreAsync(LeaderboardId, score);
            Debug.Log($"Score submitted: {result.Score} (Rank {result.Rank + 1})");
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    public async Task<List<LeaderboardEntry>> GetTopScores(int limit = 10)
    {
        if (!await EnsureReady()) return null;

        try
        {
            var options = new GetScoresOptions { Limit = limit };
            var response = await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId, options);

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

    public async Task GetPlayerScore()
    {
        if (!await EnsureReady()) return;

        try
        {
            var entry = await LeaderboardsService.Instance.GetPlayerScoreAsync(LeaderboardId);
            Debug.Log($"Your rank: #{entry.Rank + 1} — Score: {entry.Score}");
        }
        catch (RequestFailedException ex)
        {
            Debug.LogWarning($"Player has no score yet: {ex.Message}");
        }
    }
}
