using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ScreenEffectManager : MonoBehaviour
{
    [Header("Screenshake Settings")]
    [SerializeField]
    private GameObject ShakeCamera;
    private Vector3 startPos;
    private Quaternion startRotation;
    [SerializeField]
    private float resetSpeed;
    private Tweener activeTween;

    [Header("Impact Frames Settings")]
    public static ScreenEffectManager Instance { get; private set; }
    [SerializeField]
    private float impactFramesLength, blackCanvasImpactFramesLength;
    [SerializeField]
    private SpriteRenderer blackScreenCanvas;

    private void Awake() {
        if (Instance != null && Instance != this && Instance.enabled)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        if (ShakeCamera == null) {
            ShakeCamera = Camera.main.gameObject;
        }
        startPos = ShakeCamera.transform.position;
        startRotation = ShakeCamera.transform.rotation;
    }

    void Update()
    {
        if (activeTween == null || !activeTween.active) {
            ShakeCamera.transform.position = Vector3.Lerp(ShakeCamera.transform.position, startPos, resetSpeed * Time.deltaTime);
            ShakeCamera.transform.rotation = Quaternion.Lerp(ShakeCamera.transform.rotation, startRotation, resetSpeed * Time.deltaTime);
        }
    }

    public void DoImpactFrames() {
        StopCoroutine(ImpactFrames());
        Time.timeScale = 1;
        StartCoroutine(ImpactFrames());
    }

    public void DoImpactFramesWithBlackCanvas() {
        StopCoroutine(BlackCanvasImpactFrames());
        Time.timeScale = 1;
        StartCoroutine(BlackCanvasImpactFrames());
    }

    private IEnumerator ImpactFrames() {
        InputManager.Instance.LockedInput = true;
        float priorTimeScale = Time.timeScale;
        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(impactFramesLength);

        Time.timeScale = priorTimeScale;
        InputManager.Instance.LockedInput = false;
    }

    private IEnumerator BlackCanvasImpactFrames() {
        InputManager.Instance.LockedInput = true;
        float priorTimeScale = Time.timeScale;
        Time.timeScale = 0;
        blackScreenCanvas.enabled = true;

        yield return new WaitForSecondsRealtime(blackCanvasImpactFramesLength);

        Time.timeScale = priorTimeScale;
        blackScreenCanvas.enabled = false;
        InputManager.Instance.LockedInput = false;
    }
    public void Shake(float duration, float strength)
    {
        Instance.OnShake(duration, strength);
    }

    private void OnShake(float duration, float strength)
    {
        activeTween = ShakeCamera.transform.DOShakePosition(duration, strength);
        ShakeCamera.transform.DOShakeRotation(duration, strength);
    }
}
