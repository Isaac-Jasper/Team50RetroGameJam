using System.Collections;
using UnityEngine;

public class InfiniteRoundController : MonoBehaviour
{
    private int enemiesSpawned = 0;
    private int enemiesAlive = 0;

    [SerializeField] private int enemiesPerRound;
    [SerializeField] private int enemiesPerBurst;
    [SerializeField] private float timeBetweenEnemies;
    [SerializeField] private EnemyBase[] enemies;
    [SerializeField] private Transform[] spawnLocations;

    private RoundManager roundManager;

    private void Start()
    {
        roundManager = RoundManager.Instance;
    }

    public void StartRound()
    {
        enemiesSpawned = 0;
        enemiesAlive = 0;
        StartCoroutine(SpawnTimer());
    }

    private IEnumerator SpawnTimer()
{
    while (true) 
    {
        for (int i = 0; i < enemiesPerBurst; i++)
        {
            SpawnEnemy(enemies[Random.Range(0, enemies.Length)], 
                      spawnLocations[Random.Range(0, spawnLocations.Length)].position);
        }

        yield return new WaitForSeconds(timeBetweenEnemies); 
    }
}

    private void SpawnEnemy(EnemyBase enemy, Vector2 position)
    {
        EnemyBase spawnedEnemy = Instantiate(enemy, position, enemy.transform.rotation);
        enemiesSpawned++;
        enemiesAlive++;

        InfiniteEnemyDeathTracker tracker = spawnedEnemy.gameObject.AddComponent<InfiniteEnemyDeathTracker>();
        tracker.OnEnemyDestroyed += OnEnemyDestroyed;
    }

    private void OnEnemyDestroyed()
    {
        enemiesAlive--;

        if (enemiesAlive <= 0 && enemiesSpawned >= enemiesPerRound)
        {
            Debug.Log("Round completed!");
            roundManager.nextRound();
        }
    }

}
