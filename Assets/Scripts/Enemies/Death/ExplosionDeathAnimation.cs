using UnityEngine;

public class ExplosionDeathAnimation : MonoBehaviour
{
    void Start()
    {
        ScreenEffectManager.Instance.DoImpactFramesWithBlackCanvas();
    }
}
