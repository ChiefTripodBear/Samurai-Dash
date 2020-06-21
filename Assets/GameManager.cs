using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        GameSettingsButton.OnTogglePause += HandlePause;
    }

    private void OnDestroy()
    {
        GameSettingsButton.OnTogglePause -= HandlePause;
    }

    private void HandlePause(bool pause)
    {
        Time.timeScale = pause ? 0f : 1f;
    }

    public void LoadLevelSelect()
    {
        SceneManager.LoadScene("Level Select");
    }
    
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
        Pool.ClearPools();
        GameUnitManager.Clear();
    }
    
    public void LoadSettings()
    {
        SceneManager.LoadScene("Settings");
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }

    public static void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
        SceneManager.LoadScene("Game Menu UI Scene", LoadSceneMode.Additive);
    }
}
