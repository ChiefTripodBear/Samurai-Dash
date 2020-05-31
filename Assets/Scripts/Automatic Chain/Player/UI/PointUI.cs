using TMPro;
using UnityEngine;

public class PointUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _pointText;

    private void Awake()
    {
        PlayerPointManager.OnPointsChanged += UpdateScoreText;
    }

    private void OnDestroy()
    {
        PlayerPointManager.OnPointsChanged -= UpdateScoreText;
    }

    private void UpdateScoreText(int amountToChange, int currentScore)
    {
        _pointText.SetText(currentScore.ToString());
    }
}