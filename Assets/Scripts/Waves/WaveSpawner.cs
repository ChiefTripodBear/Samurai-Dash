using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private List<Wave> _waveOrder = new List<Wave>();
    [SerializeField] private bool _looping;

    private int _waveIndex = 0;

    private WaitForSeconds _spawnDelay;
    
    private List<Unit> _currentWaveUnits = new List<Unit>();
    private int _waveCompletionCount;
    private int _currentWaveEnemyCount;

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
                yield return new WaitWhile(() => _currentWaveEnemyCount < _waveCompletionCount);
                yield return new WaitForSeconds(currentWave.TimeUntilNextWave);
            }
            
            _currentWaveUnits.Clear();
        }
    }

    private IEnumerator SpawnAllEnemiesInWave(Wave currentWave)
    {
        _currentWaveEnemyCount = currentWave.SpawnOrder.Count;
        
        for (var i = 0; i < currentWave.SpawnOrder.Count; i++)
        {
            var unit = currentWave.SpawnOrder[i].Get<Unit>(null, GetRandomSpawnPosition(), Quaternion.identity);
            
            unit.Register();
            
            if (currentWave.CanStartNewWaveDuringCurrent)
            {
                unit.OnDestroyEvent += () => _waveCompletionCount++;
                _currentWaveUnits.Add(unit);
            }
            
            yield return new WaitForSeconds(currentWave.SpawnDelay);
        }
    }
    
    private Vector2 GetRandomSpawnPosition()
    {
        var screenSize = new Vector2(Camera.main.orthographicSize * Camera.main.aspect, Camera.main.orthographicSize);
        
        return new Vector2(Random.Range(-screenSize.x, screenSize.x), Random.Range(-screenSize.y, screenSize.y));
    }
}