using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitSpawner : MonoBehaviour
{
    public static event Action<PathfindingUnit> OnUnitSpawned;
    [SerializeField] private PathfindingUnit _pathfindingUnitPrefab;
    [SerializeField] private int _spawnCount;

    private void Awake()
    {
        for (var i = 0; i < _spawnCount; i++)
        {
            var unit = _pathfindingUnitPrefab.Get<PathfindingUnit>(null, GetRandomSpawnPosition(), Quaternion.identity);

            OnUnitSpawned?.Invoke(unit);
        }
    }

    private Vector2 GetRandomSpawnPosition()
    {
        var screenSize = new Vector2(Camera.main.orthographicSize * Camera.main.aspect, Camera.main.orthographicSize);
        
        return new Vector2(Random.Range(-screenSize.x, screenSize.x), Random.Range(-screenSize.y, screenSize.y));
    }
}