using System.Collections;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }

    private int round;
    private int enemiesSpawned = 0;
    private int enemiesAlive = 0;
    [SerializeField] private int totalRounds;
    [SerializeField] private int enemiesPerRound;
    [SerializeField] private float timeBetweenEnemies;
    [SerializeField] private Enemy[] enemies;
    [SerializeField] private Transform[] spawnLocations;


    private void Awake()
    {
        if (Instance != null && Instance != this && Instance.enabled)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void nextRound()
    {
        // Reset enemy count
        enemiesSpawned = 0;
        enemiesAlive = 0;
        
        if(round >= totalRounds)
        {
            // Game completed - add victory condition if needed
            Debug.Log("All rounds completed!");
            return;
        }
        else
        {
            round++;
            Debug.Log("Starting Round " + round);
            
            // If not the first round, show upgrade options
            if (round > 1 && UpgradeManager.Instance != null)
            {
                UpgradeManager.Instance.ShowUpgradeSelection();
            }
            
            StartCoroutine(SpawnEnemies());
        }
    }

    private void SpawnEnemy(Enemy enemy, Vector2 position)
    {
        Enemy spawnedEnemy = Instantiate(enemy, position, enemy.transform.rotation);
        enemiesSpawned++;
        enemiesAlive++;
        
        // Add a callback to track when enemies die
        EnemyDeathTracker tracker = spawnedEnemy.gameObject.AddComponent<EnemyDeathTracker>();
        tracker.OnEnemyDestroyed += OnEnemyDestroyed;
    }
    
    private void OnEnemyDestroyed()
    {
        enemiesAlive--;
        
        // Check if the round is complete
        if (enemiesAlive <= 0 && enemiesSpawned >= enemiesPerRound)
        {
            Debug.Log("Round completed!");
            nextRound();
        }
    }

    private IEnumerator SpawnEnemies()
    {
        while(enemiesSpawned < enemiesPerRound)
        {
            SpawnEnemy(enemies[Random.Range(0, enemies.Length)], spawnLocations[Random.Range(0, spawnLocations.Length)].position);
            yield return new WaitForSeconds(timeBetweenEnemies);
        }
    }

    void Start()
    {
        round = 0;
        nextRound();
    }
}

// Helper component to track enemy destruction
public class EnemyDeathTracker : MonoBehaviour
{
    public System.Action OnEnemyDestroyed;
    
    private void OnDestroy()
    {
        // Only trigger if the game is still running (not scene change or game exit)
        if (RoundManager.Instance != null && this.gameObject != null)
        {
            OnEnemyDestroyed?.Invoke();
        }
    }
}