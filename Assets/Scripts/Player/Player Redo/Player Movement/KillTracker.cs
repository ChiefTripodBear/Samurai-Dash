using System.Collections;
using UnityEngine;

public class KillTracker : MonoBehaviour
{
    [SerializeField] private float _fearThresholdPercent = 0.25f;
    [SerializeField] private int _currentKills;
    [SerializeField] private int _totalKills;

    private Player _player;

    private static KillTracker _instance;

    public static int CurrentKills => _instance._currentKills;
    public static int TotalKills => _instance._totalKills;

    private void Awake()
    {
        if (_instance == null) _instance = this;

        _player = FindObjectOfType<Player>();
        UnitKillHandler.UnitKillPointReached += RecordKill;
        Mover.OnArrival += HandleArrival;
    }
    
    private void OnDestroy()
    {
        UnitKillHandler.UnitKillPointReached -= RecordKill;
        Mover.OnArrival -= HandleArrival;
    }
    
    private void HandleArrival()
    {
        if (_currentKills >= GameUnitManager.UnitsOnScreenFromPercent(_fearThresholdPercent))
            GameUnitManager.PerformPostKillFear(_player);

        StartCoroutine(SetStreakToZeroWithDelay());
    }

    private IEnumerator SetStreakToZeroWithDelay()
    {
        yield return new WaitForSeconds(.5f);
        _currentKills = 0;
    }

    private void RecordKill()
    {
        _totalKills++;
        _currentKills++;
    }
}