using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // We don't use DontDestroyOnLoad here because we want a fresh manager for each game session.
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    public enum GameState { Playing, Paused, GameOver }
    public GameState CurrentState { get; private set; }

    public int Score { get; private set; }
    public event System.Action<int> OnScoreChanged;

    private void Start()
    {
        CurrentState = GameState.Playing;
        Time.timeScale = 1f;
    }

    public void AddScore(int scoreToAdd)
    {
        if (CurrentState != GameState.Playing) return;

        Score += scoreToAdd;
        OnScoreChanged?.Invoke(Score);
    }

    public void PlayerDied()
    {
        if (CurrentState == GameState.GameOver) return; // Prevent this from being called multiple times

        CurrentState = GameState.GameOver;
        Time.timeScale = 0f; 
        Debug.Log("Game Over! Final Score: " + Score);

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOverScreen(Score);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

