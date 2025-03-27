using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public void StartGame()
    {
        SceneController.Instance.InitiateSceneChange(2);
    }
}
