using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Main menu controller: the Play button loads the gameplay scene.</summary>
public class MainMenuController : MonoBehaviour
{
    [SerializeField] private int gameplaySceneIndex = 1;

    public void OnPlay()
    {
        SceneManager.LoadScene(gameplaySceneIndex);
    }
}
