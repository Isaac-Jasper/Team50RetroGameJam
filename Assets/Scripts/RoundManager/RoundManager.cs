using System.Collections;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }

    private int round;
    private int totalRounds;
    [SerializeField] private RoundController[] rounds;

    void Start()
    {
        round = 0;
        totalRounds = rounds.Length;
        nextRound();
    }

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

    public void nextRound()
    {   
        if(round >= totalRounds)
        {
            Debug.Log("All rounds completed!");
            return;
        }
        else
        {
            round++;
            Debug.Log("Starting Round " + round);
            
            if (round == 4 || round == 8 && UpgradeManager.Instance != null)
            {
                UpgradeManager.Instance.ShowUpgradeSelection();
            }
            
            rounds[round - 1].StartRound();
        }
    }
}

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