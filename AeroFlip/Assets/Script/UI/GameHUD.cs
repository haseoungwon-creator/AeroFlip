using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] Text scoreText;
    [SerializeField] Text highScoreText;

    private void Update()
    {
        if (scoreManager == null)
            return;

        if (scoreText != null)
            scoreText.text = scoreManager.CurrentScore.ToString("D6");

        if (highScoreText != null)
            highScoreText.text = scoreManager.HighScore.ToString("D6");
    }
}