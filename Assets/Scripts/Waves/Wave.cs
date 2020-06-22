using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Wave", order = 51)]
public class Wave : ScriptableObject
{
    [SerializeField] private List<EnemyUnit> _spawnOrder = new List<EnemyUnit>();
    [SerializeField] private float _spawnDelay;
    [SerializeField] private float _timeUntilNextWave;
    [SerializeField] private bool _canStartNewWaveDuringCurrent;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _destinationAfterSpawn;

    [SerializeField] private List<SubWave> _subWaves = new List<SubWave>();

    public List<SubWave> SubWaves => _subWaves;

    public List<EnemyUnit> SpawnOrder => _spawnOrder;
    public float SpawnDelay => _spawnDelay;
    public float TimeUntilNextWave => _timeUntilNextWave;
    public bool CanStartNewWaveDuringCurrent => _canStartNewWaveDuringCurrent;
    public Transform SpawnPoint => _spawnPoint;
    public Transform DestinationAfterSpawn => _destinationAfterSpawn;
}

[Serializable]
public class SubWave
{
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _postSpawnDestination;
    [SerializeField] private List<EnemyUnit> _units = new List<EnemyUnit>();
    [SerializeField] private float _spawnDelay;

    public Transform SpawnPoint => _spawnPoint;
    public Transform PostSpawnDestination => _postSpawnDestination;
    public List<EnemyUnit> Units => _units;
    public float SpawnDelay => _spawnDelay;
}