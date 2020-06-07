using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Wave", order = 51)]
public class Wave : ScriptableObject
{
    [SerializeField] private List<Unit> _spawnOrder = new List<Unit>();
    [SerializeField] private float _spawnDelay;
    [SerializeField] private float _timeUntilNextWave;
    [SerializeField] private bool _canStartNewWaveDuringCurrent;

    public List<Unit> SpawnOrder => _spawnOrder;
    public float SpawnDelay => _spawnDelay;
    public float TimeUntilNextWave => _timeUntilNextWave;
    public bool CanStartNewWaveDuringCurrent => _canStartNewWaveDuringCurrent;
}