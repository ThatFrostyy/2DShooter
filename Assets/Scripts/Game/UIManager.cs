using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic; 

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;
    public Slider healthSlider;

    [Header("Game Over Screen")]
    public GameObject gameOverPanel; 
    public TextMeshProUGUI finalScoreText;

    [Header("Card Selection Screen")]
    public GameObject cardSelectionPanel;
    public CardUI[] cardSlots;

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

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged += UpdateScore;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged -= UpdateScore;
        }
    }

    private void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (cardSelectionPanel != null)
        {
            cardSelectionPanel.SetActive(false);
        }
    }

    public void ShowCardSelection(List<CardData> cards)
    {
        cardSelectionPanel.SetActive(true);
        for (int i = 0; i < cardSlots.Length; i++)
        {
            if (i < cards.Count)
            {
                cardSlots[i].gameObject.SetActive(true);
                cardSlots[i].Display(cards[i]);
            }
            else
            {
                // Hide extra slots if you were given less than 3 cards
                cardSlots[i].gameObject.SetActive(false);
            }
        }
    }

    public void HideCardSelection()
    {
        cardSelectionPanel.SetActive(false);
    }

    public void UpdateScore(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + newScore.ToString();
        }
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void ShowGameOverScreen(int finalScore)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        if (finalScoreText != null)
        {
            finalScoreText.text = "Final Score: " + finalScore.ToString();
        }
    }

    // This public method will be called by the OnClick event of the Restart Button
    public void OnRestartButtonPressed()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }
}

