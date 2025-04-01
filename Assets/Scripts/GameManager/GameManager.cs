using System.Collections;
using DG.Tweening;
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
    [SerializeField] private Transform livesContainer;
    [SerializeField] private GameObject livesPrefab;
    [SerializeField] private GameObject blankImage;
    
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
    [SerializeField] private Button infiniteButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button tutorialButton;

    [Header("Game Settings")]
    [SerializeField] private float gameOverDelay = 2f;
    [SerializeField] private bool autoRestart = false;
    [SerializeField] private float autoRestartDelay = 3f;
    [SerializeField] private bool isEndless;

    [Header("Music")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip deathMusic;
    
    private int remainingUpgrades = 8;
    private int nextUpgradeScore = 150;
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
        InputManager.Instance.OnPause.AddListener(PauseGame);
        foreach (Button b in restartButtons)
            b.onClick.AddListener(RestartGame);
        foreach (Button b in mainMenuButtons)
            b.onClick.AddListener(ReturnToMenu);
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        if (infiniteButton != null)
            infiniteButton.onClick.AddListener(InfiniteGame);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMenu);
        if (tutorialButton != null)
            tutorialButton.onClick.AddListener(GoTutorial);
    }

    void Start()
    {
        UpdateScore(0);
        if (livesText != null)
            livesText.SetText("LIVES: ");
        if (SceneManager.GetActiveScene().buildIndex == 2 || SceneManager.GetActiveScene().buildIndex == 3)
            SoundManager.Instance.PlayMusic(SoundManager.Instance.backgroundMusic);
    }
    
    public void UpdateLivesDisplay(int lives)
    {
        if(livesContainer != null)
        {
            if(livesText != null)
            {
                if (lives > 3)
                {
                    if (lives < 10)
                        livesText.SetText("LIVES: " + lives + "x");
                    else
                        livesText.SetText("LIVES:" + lives + "x");

                    foreach (Transform child in livesContainer)
                    {
                        Destroy(child.gameObject);
                    }
                    Instantiate(blankImage, livesContainer);
                    Instantiate(blankImage, livesContainer);
                    Instantiate(livesPrefab, livesContainer);

                    return;
                }
                livesText.SetText("LIVES: ");
            }              

            foreach (Transform child in livesContainer)
            {
                Destroy(child.gameObject);
            }
            for (int i = 0; i < lives; i++)
            {
                Instantiate(livesPrefab, livesContainer);
            }
        }else
        {
            if (livesText != null)
                livesText.SetText("LIVES: " + lives);
        }
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

        if (remainingUpgrades != 0 && isEndless && score > nextUpgradeScore) {
            nextUpgradeScore = score + 100 * (9 - remainingUpgrades);
            remainingUpgrades--;
            StartCoroutine(showUpgrades());
        }
    }

    private IEnumerator showUpgrades() {
        yield return new WaitForSeconds(0.1f);
        UpgradeManager.Instance.ShowUpgradeSelection();
    }
    
    public void GameOver()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        Debug.Log("Game Over!");
        SoundManager.Instance.PauseMusic();
        
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
            SoundManager.Instance.PlaySound(deathMusic);

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
        SoundManager.Instance.PlaySound(0);
        SceneController.Instance.InitiateSceneChange(2);
    }

    private void GoTutorial()
    {
        SoundManager.Instance.PlaySound(0);
        SceneController.Instance.InitiateSceneChange(4);
    }

    private void InfiniteGame()
    {
        SoundManager.Instance.PlaySound(0);
        SceneController.Instance.InitiateSceneChange(3);
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
        SoundManager.Instance.PauseMusic();
        isPause = true;
    }

    private void ResumeGame()
    {
        Debug.Log("Unpausing");
        pausePanel.SetActive(false);
        Time.timeScale = 1;
        isPause = false;
        SoundManager.Instance.ResumeMusic();
    }

    
}