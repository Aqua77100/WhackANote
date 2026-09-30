using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TrackCompletedTests
{
    // TEST 1 (SCORE AND HIT TYPES)
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [Test]
    public void TrackCompletionStats_StoresHitCountsAndScoresCorrectly()
    {
        // Arrange & Act
        TrackCompletionStats stats = new TrackCompletionStats(
            perfects: 21, 
            greats: 32, 
            goods: 56, 
            misses: 4, 
            currentScore: 11560, 
            highScore: 10000
        );

        // Assert
        Assert.AreEqual(21, stats.Perfects);
        Assert.AreEqual(32, stats.Greats);
        Assert.AreEqual(56, stats.Goods);
        Assert.AreEqual(4, stats.Misses);
        Assert.AreEqual(11560, stats.CurrentScore);
    }

    [Test]
    [TestCase(0, StarState.Empty, StarState.Empty, StarState.Empty)]
    [TestCase(250, StarState.Half, StarState.Empty, StarState.Empty)]   // Half star 1
    [TestCase(500, StarState.Full, StarState.Empty, StarState.Empty)]   // Full star 1
    [TestCase(1000, StarState.Full, StarState.Half, StarState.Empty)]   // Full star 1, Half star 2
    [TestCase(1500, StarState.Full, StarState.Full, StarState.Empty)]   // Full star 1, Full star 2
    [TestCase(2250, StarState.Full, StarState.Full, StarState.Half)]    // Full star 1 & 2, Half star 3
    [TestCase(3000, StarState.Full, StarState.Full, StarState.Full)]    // All full stars
    
    // TEST 2 (STAR CALCULATIONS)
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public void CalculateStarStates_ReturnsCorrectStarArray_ForScoreThresholds(
        int score, 
        StarState expectedStar1, 
        StarState expectedStar2, 
        StarState expectedStar3)
    {
        // Arrange
        TrackCompletionStats stats = new TrackCompletionStats(0, 0, 0, 0, score, 0);

        // Act
        StarState[] actualStars = stats.CalculateStarStates(500, 1500, 3000);

        // Assert
        Assert.AreEqual(expectedStar1, actualStars[0], "Star 1 mismatch");
        Assert.AreEqual(expectedStar2, actualStars[1], "Star 2 mismatch");
        Assert.AreEqual(expectedStar3, actualStars[2], "Star 3 mismatch");
    }

    // TEST 3 (NEW HIGHSCORE)
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [Test]
    public void IsNewHighScore_ReturnsTrue_WhenCurrentScoreExceedsHighScore()
    {
        // Arrange
        TrackCompletionStats scoreBeaten = new TrackCompletionStats(0, 0, 0, 0, currentScore: 12000, highScore: 10000);
        TrackCompletionStats scoreNotBeaten = new TrackCompletionStats(0, 0, 0, 0, currentScore: 8000, highScore: 10000);

        // Act & Assert
        Assert.IsTrue(scoreBeaten.IsNewHighScore());
        Assert.IsFalse(scoreNotBeaten.IsNewHighScore());
    }

    // // A Test behaves as an ordinary method
    // [Test]
    // public void TrackCompletedTestsSimplePasses()
    // {
    //     // Use the Assert class to test conditions
    // }

    // // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // // `yield return null;` to skip a frame.
    // [UnityTest]
    // public IEnumerator TrackCompletedTestsWithEnumeratorPasses()
    // {
    //     // Use the Assert class to test conditions.
    //     // Use yield to skip a frame.
    //     yield return null;
    // }
}
