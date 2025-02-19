using UnityEngine;

public class DontDestroyOnLoadClass : MonoBehaviour
{
    void Awake() {
        DontDestroyOnLoad(gameObject);
    }
}
