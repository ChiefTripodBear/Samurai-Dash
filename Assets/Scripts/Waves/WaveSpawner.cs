using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private List<Wave> _waveOrder = new List<Wave>();
    [SerializeField] private bool _looping;

    private int _waveIndex = 0;

    private WaitForSeconds _spawnDelay;
    
    private List<EnemyUnit> _currentWaveUnits = new List<EnemyUnit>();
    private int _waveCompletionCount;

    private IEnumerator Start()
    {
        do
        {
            yield return StartCoroutine(SpawnAllWaves());
        } 
        while (_looping);
    }

    private IEnumerator SpawnAllWaves()
    {
        yield return new WaitForSeconds(1f);
        for (_waveIndex = 0; _waveIndex < _waveOrder.Count; _waveIndex++)
        {
            var currentWave = _waveOrder[_waveIndex];
            yield return StartCoroutine(SpawnAllEnemiesInWave(currentWave));
            
            if (currentWave.CanStartNewWaveDuringCurrent)
            {
                yield return new WaitForSeconds(currentWave.TimeUntilNextWave);
            }
            else
            {
                yield return new WaitWhile(() => _currentWaveUnits.Count > 0);
                yield return new WaitForSeconds(currentWave.TimeUntilNextWave);
            }
            
            _currentWaveUnits.Clear();
        }
    }

    private IEnumerator SpawnAllEnemiesInWave(Wave currentWave)
    {
        for (var i = 0; i < currentWave.SpawnOrder.Count; i++)
        {
            var unit = currentWave.SpawnOrder[i].Get<EnemyUnit>(null, currentWave.SpawnPoint.position, Quaternion.identity);
            
            unit.Register();
            unit.MoveFromSpawn(currentWave.DestinationAfterSpawn.position);
            
            if (!currentWave.CanStartNewWaveDuringCurrent)
            {
                _currentWaveUnits.Add(unit);
                unit.KillHandler.OnDeath += () => _currentWaveUnits.Remove(unit);
            }

            yield return new WaitForSeconds(currentWave.SpawnDelay);
        }
    }
}