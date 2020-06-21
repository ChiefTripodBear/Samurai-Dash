using System;
using TMPro;
using UnityEngine;

public class GameSettingsButton : MonoBehaviour
{
    [SerializeField] private GameObject _pauseScreenButtonsHolder;
    [SerializeField] private string _activeText = "Return";
    [SerializeField] private string _inactiveText = "Settings";
    [SerializeField] private TMP_Text _settingsButtonText;
    
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
        
        _settingsButtonText.SetText(_pauseScreenActive ? _activeText : _inactiveText);

        _pauseScreenButtonsHolder.SetActive(_pauseScreenActive);
    }
}
