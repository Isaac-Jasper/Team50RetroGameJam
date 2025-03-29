using System.Collections;
using UnityEngine;

public class RoundController : MonoBehaviour
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
        for(int i = 0; i < enemies.Length; i++)
        {
            EnemyBase temp = enemies[i];
            int r = Random.Range(i, enemies.Length);
            enemies[i] = enemies[r];
            enemies[r] = temp;
        }

        enemiesSpawned = 0;
        enemiesAlive = 0;
        StartCoroutine(SpawnTimer());
    }

    private IEnumerator SpawnTimer()
    {
        while (enemiesSpawned < enemiesPerRound)
        {
            if (enemiesPerBurst > enemiesPerRound - enemiesSpawned) enemiesPerBurst = enemiesPerRound - enemiesSpawned;
            for (int i = 0; i < enemiesPerBurst; i++)
                SpawnEnemy(enemies[enemiesSpawned], spawnLocations[Random.Range(0, spawnLocations.Length)].position);

            yield return new WaitForSeconds(timeBetweenEnemies);
        }
    }

    private void SpawnEnemy(EnemyBase enemy, Vector2 position)
    {
        EnemyBase spawnedEnemy = Instantiate(enemy, position, enemy.transform.rotation);
        enemiesSpawned++;
        enemiesAlive++;

        EnemyDeathTracker tracker = spawnedEnemy.gameObject.AddComponent<EnemyDeathTracker>();
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
