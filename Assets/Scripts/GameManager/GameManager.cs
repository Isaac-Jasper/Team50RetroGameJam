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
    
    [Header("Game Over/Win/Pause UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private Button[] restartButtons;
    [SerializeField] private Button[] mainMenuButtons;
    [SerializeField] private Button resumeButton;

    [Header("Start Menu UI")]
    [SerializeField] private Button startButton;

    [Header("Game Settings")]
    [SerializeField] private float gameOverDelay = 2f;
    [SerializeField] private bool autoRestart = false;
    [SerializeField] private float autoRestartDelay = 3f;
    
    private int currentScore = 0;
    private bool isGameOver = false;
    public bool isPause = false;

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
        InputManager.Instance.OnRestart.AddListener(RestartGame);
        InputManager.Instance.OnPause.AddListener(PauseGame);
        foreach (Button b in restartButtons)
            b.onClick.AddListener(RestartGame);
        foreach (Button b in mainMenuButtons)
            b.onClick.AddListener(ReturnToMenu);
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);
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

    private void PauseGame()
    {
        if (isGameOver)
        {
            Debug.Log("????");
            return;
        }
        if (isPause)
        {
            Debug.Log("Unpausing");
            ResumeGame();
            return;
        }
        Debug.Log("Pausing");
        Time.timeScale = 0;
        pausePanel.SetActive(true);
        isPause = true;
    }

    private void ResumeGame()
    {
        Debug.Log("Unpausing");
        pausePanel.SetActive(false);
        Time.timeScale = 1;
        isPause = false;
    }

    
}