using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        playButton.onClick.AddListener(OnPlayButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
        
        playButton.Select();
    }

    private void OnPlayButtonClicked()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    private void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}
