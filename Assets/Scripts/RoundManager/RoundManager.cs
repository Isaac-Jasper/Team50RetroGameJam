using System.Collections;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }

    private int round;
    private int enemiesSpawned = 0;
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
        //TODO: Add ui element to show that round is complete
        if(round >= totalRounds)
        {
            //TODO
        }else
        {
            round++;
            StartCoroutine(SpawnEnemies());
        }
    }

    private void SpawnEnemy(Enemy enemy, Vector2 position)
    {
        Instantiate(enemy, position, enemy.transform.rotation);
        enemiesSpawned++;
    }

    private IEnumerator SpawnEnemies()
    {
        while(enemiesSpawned < enemiesPerRound)
        {
            SpawnEnemy(enemies[Random.Range(0, enemies.Length - 1)], spawnLocations[Random.Range(0, spawnLocations.Length -1)].position);
            yield return new WaitForSeconds(timeBetweenEnemies);
        }
    }

    void Start()
    {
        round = 0;
        nextRound();
    }

    void Update()
    {
        
    }
}
