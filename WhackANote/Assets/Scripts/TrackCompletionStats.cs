using UnityEngine;

public enum StarState
{
    Empty,
    Half,
    Full
}

public class TrackCompletionStats
{
    public int Perfects { get; private set; }
    public int Greats { get; private set; }
    public int Goods { get; private set; }
    public int Misses { get; private set; }
    
    public int CurrentScore { get; private set; }
    public int HighScore { get; private set; }

    // CODE FOR: TEST 1 (SCORE AND HIT TYPES)
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public TrackCompletionStats(int perfects, int greats, int goods, int misses, int currentScore, int highScore)
    {
        Perfects = perfects;
        Greats = greats;
        Goods = goods;
        Misses = misses;
        CurrentScore = currentScore;
        HighScore = highScore;
    }

    // CODE FOR: TEST 2 (STAR CALCULATIONS)
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public StarState[] CalculateStarStates(int oneStarThreshold = 500, int twoStarThreshold = 1500, int threeStarThreshold = 3000)
    {
        StarState[] stars = new StarState[3] { StarState.Empty, StarState.Empty, StarState.Empty };

        // Midpoints for half stars
        int halfOne = oneStarThreshold / 2; // 250
        int halfTwo = oneStarThreshold + (twoStarThreshold - oneStarThreshold) / 2; // 1000
        int halfThree = twoStarThreshold + (threeStarThreshold - twoStarThreshold) / 2; // 2250

        // Star 1
        if (CurrentScore >= oneStarThreshold) stars[0] = StarState.Full;
        else if (CurrentScore >= halfOne) stars[0] = StarState.Half;

        // Star 2
        if (CurrentScore >= twoStarThreshold) stars[1] = StarState.Full;
        else if (CurrentScore >= halfTwo) stars[1] = StarState.Half;

        // Star 3
        if (CurrentScore >= threeStarThreshold) stars[2] = StarState.Full;
        else if (CurrentScore >= halfThree) stars[2] = StarState.Half;

        return stars;
    }

    // CODE FOR:  TEST 3 (NEW HIGHSCORE)
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public bool IsNewHighScore()
    {
        return CurrentScore > HighScore;
    }
}