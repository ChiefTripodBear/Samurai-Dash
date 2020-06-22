using TMPro;
using UnityEngine;

public class KillsDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text _currentStreakText;
    [SerializeField] private TMP_Text _totalKillsText;
    private void Update()
    {
        _currentStreakText.SetText("Streak: " + KillTracker.CurrentStreak);
        _totalKillsText.SetText("Kills: " + KillTracker.TotalKills);
    }
}