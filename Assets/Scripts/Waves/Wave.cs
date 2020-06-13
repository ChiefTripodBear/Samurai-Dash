using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Wave", order = 51)]
public class Wave : ScriptableObject
{
    [SerializeField] private List<EnemyUnit> _spawnOrder = new List<EnemyUnit>();
    [SerializeField] private float _spawnDelay;
    [SerializeField] private float _timeUntilNextWave;
    [SerializeField] private bool _canStartNewWaveDuringCurrent;

    public List<EnemyUnit> SpawnOrder => _spawnOrder;
    public float SpawnDelay => _spawnDelay;
    public float TimeUntilNextWave => _timeUntilNextWave;
    public bool CanStartNewWaveDuringCurrent => _canStartNewWaveDuringCurrent;
}