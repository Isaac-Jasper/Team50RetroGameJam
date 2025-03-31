using System.Collections;
using UnityEngine;

public class DuckDeathAnimation : MonoBehaviour
{
    [SerializeField]
    Animator animator;
    [SerializeField]
    GameObject whiteSquareObject;
    [SerializeField]
    float hitWaitTime, fallSpeed;
    void Start()
    {
        StartCoroutine(DeathAnimation());
    }

    private IEnumerator DeathAnimation() {
        whiteSquareObject.SetActive(true);

        animator.Play("EnemyHit");
        ScreenEffectManager.Instance.DoImpactFramesWithBlackCanvas();

        yield return new WaitForSecondsRealtime(ScreenEffectManager.Instance.BlackCanvasImpactFramesLength);
        whiteSquareObject.SetActive(false);
        
        yield return new WaitForSeconds(hitWaitTime);

        animator.Play("EnemyDeath");
        while(transform.position.y > -10) {
            yield return null;
            transform.position = transform.position - new Vector3(0,fallSpeed*Time.deltaTime,0);
        }
        Destroy(gameObject);
    }
}
