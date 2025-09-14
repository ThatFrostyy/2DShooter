using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    [Header("Leveling Configuration")]
    [Tooltip("A list of all possible card upgrades the player can get.")]
    public List<CardData> allCardUpgrades = new();
    public int scoreForFirstLevel = 100;
    public float levelUpMultiplier = 1.5f;

    public enum GameState { Playing, Paused, LevelUp, GameOver } 
    public GameState CurrentState { get; private set; }

    public int Score { get; private set; }
    public event System.Action<int> OnScoreChanged;

    private int currentPlayerLevel = 1;
    private int scoreForNextLevel;

    private GameObject playerObject;
    private PlayerController playerController;
    private Health playerHealth;
    private PlayerShooting playerShooting;


    private void Start()
    {
        CurrentState = GameState.Playing;
        Time.timeScale = 1f;

        scoreForNextLevel = scoreForFirstLevel;

        playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerController = playerObject.GetComponent<PlayerController>();
            playerHealth = playerObject.GetComponent<Health>();
            playerShooting = playerObject.GetComponent<PlayerShooting>();
        }
        else
        {
            Debug.LogError("Player object is not assigned in the GameManager!", this);
        }
    }

    public void AddScore(int scoreToAdd)
    {
        if (CurrentState != GameState.Playing)
        {
            return;
        }

        Score += scoreToAdd;
        OnScoreChanged?.Invoke(Score);

        CheckForLevelUp();
    }

    private void CheckForLevelUp()
    {
        while (Score >= scoreForNextLevel)
        {
            currentPlayerLevel++;
            int scoreRequired = scoreForNextLevel; // store for display/calculation if needed
            scoreForNextLevel = Mathf.FloorToInt(scoreForNextLevel * levelUpMultiplier);

            TriggerLevelUp();
        }
    }

    private void TriggerLevelUp()
    {
        CurrentState = GameState.LevelUp;
        Time.timeScale = 0f; 

        // Get 3 unique random cards
        List<CardData> chosenCards = allCardUpgrades.OrderBy(x => Random.value).Take(3).ToList();

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowCardSelection(chosenCards);
        }
    }

    public void ApplyUpgrade(CardData chosenCard)
    {
        switch (chosenCard.upgradeType)
        {
            case CardData.UpgradeType.IncreaseMoveSpeed:
                playerController.IncreaseSpeed(chosenCard.upgradeValue);
                break;
            case CardData.UpgradeType.IncreaseMaxHealth:
                playerHealth.IncreaseMaxHealth(chosenCard.upgradeValue);
                break;
            case CardData.UpgradeType.IncreaseFireRate:
                playerShooting.IncreaseFireRate(chosenCard.upgradeValue);
                break;
            case CardData.UpgradeType.ChangeWeapon:
                playerShooting.ChangeWeapon(chosenCard.weaponData);
                break;
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideCardSelection();
        }

        CurrentState = GameState.Playing;
        Time.timeScale = 1f;
    }

    public void PlayerDied()
    {
        if (CurrentState == GameState.GameOver) return;

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