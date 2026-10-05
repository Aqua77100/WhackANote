using TMPro;
using UnityEngine;
using Unity.Services.Authentication;

public class LeaderboardPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Transform rowContainer;        // Scroll View > Viewport > Content
    [SerializeField] private LeaderboardRowUI rowPrefab;
    [SerializeField] private LeaderboardRowUI yourRankRow;  // pinned row outside the scroll area
    [SerializeField] private int maxEntries = 50;

    // Called by each track's Leaderboard button with that track's ID (e.g. "Circus")
    public async void Open(string trackId)
    {
        gameObject.SetActive(true);
        titleText.text = $"{trackId} Leaderboard";
        statusText.text = "Loading...";
        yourRankRow.gameObject.SetActive(false);
        ClearRows();

        try
        {
            var entries = await LeaderboardManager.Instance.GetTopScores(trackId, maxEntries);

            if (entries == null)
            {
                statusText.text = "Couldn't load the leaderboard.";
                return;
            }

            statusText.text = entries.Count == 0 ? "No scores yet. Be the first!" : "";

            string myId = AuthenticationService.Instance.PlayerId;

            foreach (var entry in entries)
            {
                var row = Instantiate(rowPrefab, rowContainer);
                row.Set(entry.Rank + 1, entry.PlayerName ?? entry.PlayerId, entry.Score, entry.PlayerId == myId);
            }

            // Pinned row so you can always see your own position, even when scrolled away
            var mine = await LeaderboardManager.Instance.GetPlayerScore(trackId);
            if (mine != null)
            {
                yourRankRow.gameObject.SetActive(true);
                yourRankRow.Set(mine.Rank + 1, mine.PlayerName ?? mine.PlayerId, mine.Score, true);
            }

            // New text objects were just created, so re-apply the dyslexia font setting
            if (DyslexiaFontManager.Instance != null)
            {
                DyslexiaFontManager.Instance.ApplyFontToScene();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
            statusText.text = "Couldn't load the leaderboard.";
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void ClearRows()
    {
        foreach (Transform child in rowContainer)
        {
            Destroy(child.gameObject);
        }
    }
}