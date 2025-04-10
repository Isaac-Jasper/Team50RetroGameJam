using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }

    private int round;
    private int totalRounds;
    [SerializeField] private RoundController[] rounds;
    [SerializeField] private GameObject roundCounterPanel;
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private float timeBetweenRounds;
    [SerializeField] private Transform duckTacker;

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
    }

    public void nextRound()
    {   
        if(round >= totalRounds)
        {
            GameManager.Instance.GameWon();
            return;
        }
        else
        {
            round++;

            Debug.Log("Starting Round " + round);

            StartCoroutine(showRoundCounter());
        }
    }

    private IEnumerator showRoundCounter()
    {
        if (round == 4 || round == 8 && UpgradeManager.Instance != null)
        {
            yield return new WaitForSeconds(timeBetweenRounds);
            UpgradeManager.Instance.ShowUpgradeSelection();
        }
            

        yield return new WaitForSeconds(timeBetweenRounds/2);

        roundText.SetText("ROUND " + round);
        roundCounterPanel.SetActive(true);
        updateDuckTrackerMax();
        yield return new WaitForSeconds(timeBetweenRounds);
        roundCounterPanel.SetActive(false);

        yield return new WaitForSeconds(timeBetweenRounds/2);

        rounds[round - 1].StartRound();
    }

    private void updateDuckTrackerMax()
    {
        int maxEnemies = rounds[round - 1].getMaxEnemies();
        
        for (int i = 0; i < 15; i++)
        {
            RawImage duckSprite = duckTacker.GetChild(i).GetComponent<RawImage>();
            if (i < maxEnemies)
                duckSprite.color = Color.white;
            else
                duckSprite.color = Color.gray;

        }
    }

    public void updateDuckTrackerHit(int ducksHit)
    {
        RawImage duckSprite = duckTacker.GetChild(ducksHit - 1).GetComponent<RawImage>();
        duckSprite.color = new Color(1f, 0.3f, 0.3f, 1f);
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