using UnityEngine;

public class KillTracker : MonoBehaviour
{
    [SerializeField] private float _fearThresholdPercent = 0.25f;
    [SerializeField] private int _currentStreak;
    [SerializeField] private int _totalKills;

    private Player _player;

    private static KillTracker _instance;

    public static int CurrentStreak => _instance._currentStreak;
    public static int TotalKills => _instance._totalKills;
    
    private void Awake()
    {
        if (_instance == null) _instance = this;

        _player = FindObjectOfType<Player>();
        UnitKillHandler.UnitKillPointReached += RecordKill;
        PlayerMover.OnArrival += HandleArrival;
        PlayerMover.OnFirstMove += HandleStart;
    }

    private void OnDestroy()
    {
        UnitKillHandler.UnitKillPointReached -= RecordKill;
        PlayerMover.OnArrival -= HandleArrival;
        PlayerMover.OnFirstMove -= HandleStart;
    }

    private void HandleStart()
    {
        _currentStreak = 0;
    }

    private void HandleArrival()
    {
        if (_currentStreak >= GameUnitManager.UnitsOnScreenFromPercent(_fearThresholdPercent))
            GameUnitManager.PerformPostKillFear(_player);
    }

    private void RecordKill()
    {
        _totalKills++;
        _currentStreak++;
    }
}