using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardRowUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Image background;
    [SerializeField] private Color normalColor = new Color(1f, 1f, 1f, 0.15f);
    [SerializeField] private Color playerColor = new Color(1f, 0.85f, 0.2f, 0.6f);

    public void Set(int rank, string playerName, double score, bool isPlayer)
    {
        rankText.text = $"#{rank}";
        nameText.text = playerName;
        scoreText.text = ((int)score).ToString();

        if (background != null)
        {
            background.color = isPlayer ? playerColor : normalColor;
        }
    }
}