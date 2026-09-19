using UnityEngine;

public enum GameStates
{
    Ready,
    Playing,
    GameOver
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] WorldMovement worldMoveMent;
    [SerializeField] ScoreManager scoreManager;
    public GameStates CurrentState {  get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        CurrentState = GameStates.Ready;
    }

    private void Start()
    {
        SetGameState(GameStates.Playing);
    }

    public void StartGame()
    {
        SetGameState(GameStates.Playing);
    }

    public void GameOver()
    {
        SetGameState(GameStates.GameOver);
    }

    private void SetGameState(GameStates state)
    {
        CurrentState = state;

        switch (state)
        {
            case GameStates.Ready:
                worldMoveMent.SetMovement(false);
                worldMoveMent.ResetPosition();
                scoreManager.StopScoring();
                scoreManager.ResetScore();
                break;
            case GameStates.Playing:
                worldMoveMent.SetMovement(true);
                scoreManager.StartScoring();
                break;
            case GameStates.GameOver:
                worldMoveMent.SetMovement(false);
                scoreManager.StopScoring();
                worldMoveMent.ResetPosition();
                scoreManager.SaveHighScore();
                break;
        }
    }
}
