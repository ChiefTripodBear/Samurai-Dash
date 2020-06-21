using System;
using UnityEngine;

public class GameSettingsButton : MonoBehaviour
{
    [SerializeField] private GameObject _pauseScreenButtonsHolder;
    
    public static event Action<bool> OnTogglePause;

    private bool _pauseScreenActive;

    private void Awake()
    {
        _pauseScreenButtonsHolder.SetActive(false);
    }

    public void ToggleSettings()
    {
        _pauseScreenActive = !_pauseScreenActive;

        OnTogglePause?.Invoke(_pauseScreenActive);

        _pauseScreenButtonsHolder.SetActive(_pauseScreenActive);
    }
}
