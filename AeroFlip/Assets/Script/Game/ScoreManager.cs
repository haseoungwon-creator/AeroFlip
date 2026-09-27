using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] WorldMovement worldMovement;
    [SerializeField] PlayerMode playerMode;
    [SerializeField] int nearMissScore = 5;
    [SerializeField] float distanceScoreMultiplier = 0.05f;

    public int CurrentScore { get; private set; }
    public int HighScore { get; private set; }

    private const string HighScoreKey = "HighScore";

    private float accumulatedDistance;
    private bool scoringEnabled;
    private bool isRewinding;

    private void Awake()
    {
        LoadHighScore();
    }

    private void Update()
    {
        if (isRewinding)
            return;

        if (!scoringEnabled || worldMovement == null)
            return;

        AddDistanceScore(
            worldMovement.CurrentSpeed * Time.deltaTime);
    }

    private void AddDistanceScore(float distance)
    {
        accumulatedDistance +=
            distance * distanceScoreMultiplier;

        int score =
            Mathf.FloorToInt(accumulatedDistance);

        if (score <= 0)
            return;

        accumulatedDistance -= score;

        CurrentScore += score;

        UpdateHighScore();
    }

    public void AddNearMiss()
    {
        if (isRewinding)
            return;

        if (!scoringEnabled ||
            playerMode == null ||
            !playerMode.Is3D())
            return;

        CurrentScore += nearMissScore;

        UpdateHighScore();
    }

    public void SetRewinding(bool value)
    {
        isRewinding = value;
    }

    public void StartScoring()
    {
        scoringEnabled = true;
    }

    public void StopScoring()
    {
        scoringEnabled = false;
    }

    public void ResetScore()
    {
        CurrentScore = 0;
        accumulatedDistance = 0f;
    }

    public void SaveHighScore()
    {
        PlayerPrefs.SetInt(
            HighScoreKey,
            HighScore);

        PlayerPrefs.Save();
    }

    private void LoadHighScore()
    {
        HighScore =
            PlayerPrefs.GetInt(
                HighScoreKey,
                0);
    }

    private void UpdateHighScore()
    {
        if (CurrentScore > HighScore)
            HighScore = CurrentScore;
    }
}