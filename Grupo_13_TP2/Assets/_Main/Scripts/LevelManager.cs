using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private Button leaveButton;

    private void Awake()
    {
        leaveButton.onClick.AddListener(OnLeaveButtonClicked);
    }

    private void OnLeaveButtonClicked()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
