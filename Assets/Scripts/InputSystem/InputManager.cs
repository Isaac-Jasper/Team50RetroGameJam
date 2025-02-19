using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour 
{
    public static InputManager Instance { get; private set; }

    public UnityEvent OnRestart = new();
    public UnityEvent OnPause = new();
    public UnityEvent OnFire = new();

    public bool LockedInput { get; set; }
    private void Awake() {
        if (Instance != null && Instance != this && Instance.enabled) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    /// <param name="name"></param>
    /// <returns>True if the input can be done at that location and time</returns>
    public bool CheckIfCanInput() {
        if (Time.timeScale == 0) return false;
        if (LockedInput) return false;

        return true;
    }

    public void Update() {
        if (Input.GetButtonDown("Restart")) {
            RestartPerformed();
        }

        if (Input.GetButtonDown("Pause")) {
            PausePerformed();
        }

        if (Input.GetButtonDown("Fire")) {
            FirePerformed();
        }

        
    }

    private void PausePerformed() {
        Debug.Log("Pause Performed");
        if (!CheckIfCanInput()) return;

        //GameSoundController.i.PlayMusic(GameSoundController.Sound.Music_TestMusic1);

        OnPause?.Invoke();
    }

    private void RestartPerformed() {
        Debug.Log("Restart Performed");
        if (!CheckIfCanInput()) return;

        //GameSoundController.i.PlayMusic(GameSoundController.Sound.Music_TestMusic2);

        OnRestart?.Invoke();
    }

    private void FirePerformed() {
        Debug.Log("Fire Performed");
        if (!CheckIfCanInput()) return;

        OnFire?.Invoke();
    }
}
