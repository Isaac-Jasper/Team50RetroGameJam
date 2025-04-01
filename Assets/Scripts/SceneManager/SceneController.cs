using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

//contains a basic black screen fade out and fade in
public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

    [SerializeField]
    private float transitionSpeed;
    [SerializeField]
    private float sceneStartTransitionDelay;

    [SerializeField]
    private CanvasGroup fadeGroupPrefab;
    private CanvasGroup fadeGroup;


    private void Awake() {
        if (Instance != null && Instance != this && Instance.enabled) {
            Destroy(gameObject);
            return;
        } else {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void Start() {
        InputManager.Instance.OnRestart.AddListener(RestartScene);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        //SoundManager.Instance.PlayMusic(SoundManager.Instance.backgroundMusic);
        InputManager.Instance.LockedInput = true;
        //Dont Destroy On Load Objects are stored in scene 0, if we are in scene 0 immediatly go to the first scene of the game
        //Scene 0 could also have intro things like a splash screen or cutscene.
        if (SceneManager.GetActiveScene().buildIndex == 0) {
            SceneManager.LoadScene(1);
            return;
        }
        Debug.Log("SceneLoaded");
        initializeObjects();
        StartCoroutine(StartScene());
    }

    private void initializeObjects() {
        //initialize any on scene start objects here
        if (fadeGroup == null)
        {
            fadeGroup = Instantiate(fadeGroupPrefab);
        }
    }

    public void InitiateSceneChange(int scene)
    {
        StartCoroutine(ChangeScene(scene));
    }

    public IEnumerator ChangeScene(int scene) {
        InputManager.Instance.LockedInput = true;
        float time = 0;
        while (fadeGroup.alpha > 0.98) {
            fadeGroup.alpha = Mathf.Lerp(0, 1, time);
            time += Time.deltaTime*transitionSpeed/100;
            yield return null;
        }
        fadeGroup.alpha = 1;
        SoundManager.Instance.StopMusic();
        SceneManager.LoadScene(scene);
    }

    private void RestartScene() {
        StartCoroutine(ChangeScene(SceneManager.GetActiveScene().buildIndex));
    }

    private IEnumerator StartScene() {
        InputManager.Instance.LockedInput = true;
        fadeGroup.alpha = 1;

        yield return new WaitForSeconds(sceneStartTransitionDelay);
        
        float time = 0;
        while (fadeGroup.alpha > 0.02) {
            fadeGroup.alpha = Mathf.Lerp(1, 0, time);
            time += Time.deltaTime*transitionSpeed/100;
            yield return null;
        }
        fadeGroup.alpha = 0;
        InputManager.Instance.LockedInput = false;
    }
}
