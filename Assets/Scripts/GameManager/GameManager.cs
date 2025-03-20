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
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private Button restartButton;
    
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
        DontDestroyOnLoad(gameObject);
        
        // Initialize UI
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
            
        // Setup button listener
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
    }

    void Start()
    {
        UpdateScore(0);
    }
    
    public void UpdateLivesDisplay(int lives)
    {
        if (livesText != null)
            livesText.SetText("Lives: " + lives);
    }
    
    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScore(currentScore);
    }
    
    private void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.SetText("Score: " + score);
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
            gameOverPanel.SetActive(true);
            
            if (finalScoreText != null)
                finalScoreText.SetText("Final Score: " + currentScore);
        }
    }
    
    public void RestartGame()
    {
        isGameOver = false;
        currentScore = 0;
        
        // Hide game over UI - ensure this runs before scene change
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}