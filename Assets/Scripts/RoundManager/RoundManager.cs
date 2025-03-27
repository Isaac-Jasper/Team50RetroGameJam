using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }

    [System.Serializable]
    public class RoundConfiguration
    {
        public string roundName;
        public Enemy[] enemyTypes;
        public int totalEnemiesToSpawn;
        public int enemiesPerWave;
        public float timeBetweenWaves;
        public Transform[] spawnPoints;
    }

    [SerializeField] private RoundConfiguration[] rounds;
    [SerializeField] private float enemyCheckInterval = 0.5f;
    
    private int currentRoundIndex = 0;
    private bool isRoundInProgress = false;

    public event System.Action<RoundConfiguration> OnRoundStart;
    public event System.Action<RoundConfiguration> OnRoundComplete;

    void Awake()
    {
        // Singleton setup with more robust initialization
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        StartRound();
    }

    public void StartRound()
    {
        if (currentRoundIndex >= rounds.Length)
        {
            HandleGameCompletion();
            return;
        }

        if (isRoundInProgress)
        {
            Debug.LogWarning("Cannot start a new round while another is in progress.");
            return;
        }

        RoundConfiguration currentRound = rounds[currentRoundIndex];
        OnRoundStart?.Invoke(currentRound);
        StartCoroutine(ExecuteRound(currentRound));
    }

    private void HandleGameCompletion()
    {
        Debug.Log("All rounds completed!");
        // Implement game completion logic - could be triggering end screen, final score, etc.
    }

    private IEnumerator ExecuteRound(RoundConfiguration roundConfig)
    {
        isRoundInProgress = true;
        int enemiesSpawned = 0;
        List<Enemy> availableEnemies = new List<Enemy>(roundConfig.enemyTypes);

        while (enemiesSpawned < roundConfig.totalEnemiesToSpawn)
        {
            int enemiesToSpawnThisWave = Mathf.Min(
                roundConfig.enemiesPerWave, 
                roundConfig.totalEnemiesToSpawn - enemiesSpawned
            );

            for (int i = 0; i < enemiesToSpawnThisWave; i++)
            {
                // Replenish enemy list if empty
                if (availableEnemies.Count == 0)
                {
                    availableEnemies = new List<Enemy>(roundConfig.enemyTypes);
                }

                // Randomly select and spawn enemy
                int randomIndex = Random.Range(0, availableEnemies.Count);
                Enemy enemyToSpawn = availableEnemies[randomIndex];
                availableEnemies.RemoveAt(randomIndex);

                // Spawn at random location
                Vector2 spawnLocation = roundConfig.spawnPoints[Random.Range(0, roundConfig.spawnPoints.Length)].position;
                Instantiate(enemyToSpawn, spawnLocation, Quaternion.identity);

                enemiesSpawned++;
            }

            yield return new WaitForSeconds(roundConfig.timeBetweenWaves);
        }

        yield return StartCoroutine(WaitForRoundCompletion(roundConfig));
    }

    private IEnumerator WaitForRoundCompletion(RoundConfiguration roundConfig)
    {
        while (FindObjectsOfType<Enemy>().Length > 0)
        {
            yield return new WaitForSeconds(enemyCheckInterval);
        }

        isRoundInProgress = false;
        OnRoundComplete?.Invoke(roundConfig);

        // Trigger upgrade selection 
        UpgradeManager.Instance?.ShowUpgradeSelection();

        // Move to next round
        currentRoundIndex++;
        StartRound();
    }

    // Optional: Method to force start a specific round
    public void SetRound(int roundIndex)
    {
        if (roundIndex >= 0 && roundIndex < rounds.Length)
        {
            currentRoundIndex = roundIndex;
            StartRound();
        }
    }
}