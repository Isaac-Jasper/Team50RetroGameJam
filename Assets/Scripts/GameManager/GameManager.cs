using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI scoreText;
    
    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private Button[] restartButtons;
    [SerializeField] private Button[] mainMenuButtons;

    [Header("Start Menu UI")]
    [SerializeField] private Button startButton;

    [Header("Game Settings")]
    [SerializeField] private float gameOverDelay = 2f;
    [SerializeField] private bool autoRestart = false;
    [SerializeField] private float autoRestartDelay = 3f;
    
    private int currentScore = 0;
    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance != null && Instance != this && Instance.enabled)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        // Initialize UI
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (winPanel != null) 
            winPanel.SetActive(false);

        addListeners();
    }

    private void addListeners()
    {
        foreach (Button b in restartButtons)
            b.onClick.AddListener(RestartGame);
        foreach (Button b in mainMenuButtons)
            b.onClick.AddListener(ReturnToMenu);
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);
    }

    void Start()
    {
        UpdateScore(0);
    }
    
    public void UpdateLivesDisplay(int lives)
    {
        if (livesText != null)
            livesText.SetText("LIVES: " + lives);
    }
    
    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScore(currentScore);
    }
    
    private void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.SetText("SCORE: " + score);
    }
    
    public void GameOver()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        Debug.Log("Game Over!");
        
        // Delayed game over UI display
        Invoke("ShowGameOverUI", gameOverDelay);
        
        // Auto restart if enabled
        if (autoRestart)
            Invoke("RestartGame", autoRestartDelay);
    }
    
    private void ShowGameOverUI()
    {
        if (gameOverPanel != null)
        {
            Time.timeScale = 0;
            gameOverPanel.SetActive(true);
            
            if (finalScoreText != null)
                finalScoreText.SetText("FINAL SCORE: " + currentScore);
        }
    }

    public void GameWon()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("Final Round Completed!");

        // Delayed game over UI display
        Invoke("ShowWinUI", gameOverDelay);
    }

    private void ShowWinUI()
    {
        if (winPanel != null)
        {
            Time.timeScale = 0;
            winPanel.SetActive(true);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        isGameOver = false;
        currentScore = 0;

        InputManager.Instance.OnRestart?.Invoke();
    }

    private void StartGame()
    {
        SceneController.Instance.InitiateSceneChange(2);
    }

    private void ReturnToMenu()
    {
        Time.timeScale = 1;
        isGameOver = false;
        currentScore = 0;

        SceneController.Instance.InitiateSceneChange(1);
    }

    
}