using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private List<Wave> _waveOrder = new List<Wave>();
    [SerializeField] private bool _looping;

    private int _waveIndex = 0;

    private WaitForSeconds _spawnDelay;
    
    private readonly List<EnemyUnit> _currentWaveUnits = new List<EnemyUnit>();
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
            yield return StartCoroutine(SpawnAllEnemiesInSubWaves(currentWave));
            
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

    private IEnumerator SpawnAllEnemiesInSubWaves(Wave currentWave)
    {
        var max = currentWave.SubWaves.Max(t => t.Units.Count);
        
        for (var i = 0; i < max; i++)
        {
            for (var j = 0; j < currentWave.SubWaves.Count; j++)
            {
                var currentSubWave = currentWave.SubWaves[j];

                if (i > currentSubWave.Units.Count - 1)
                    continue;

                var unit = currentSubWave.Units[i].Get<EnemyUnit>(null, currentSubWave.SpawnPoint.position, Quaternion.identity);
            
                unit.Register();
                unit.MoveFromSpawn(currentSubWave.PostSpawnDestination.position);
            
                if (!currentWave.CanStartNewWaveDuringCurrent)
                {
                    _currentWaveUnits.Add(unit);
                    unit.KillHandler.OnDeath += () => _currentWaveUnits.Remove(unit);
                }
            
                yield return new WaitForSeconds(currentSubWave.SpawnDelay);
            }
        }
    }
}