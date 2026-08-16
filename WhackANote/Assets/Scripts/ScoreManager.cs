using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{

    public static ScoreManager Instance {
        get;
        private set;
    }

    public TextMeshProUGUI scoreText;
    private int currentScore = 0;

    private void Awake(){
        if (Instance == null){
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (scoreText == null){
            scoreText = Object.FindAnyObjectByType<TextMeshProUGUI>();
        }
    }

    public void AddScore(int points){
        currentScore += points;

        if(scoreText != null){
            scoreText.text = currentScore.ToString();
        }
    }
}
