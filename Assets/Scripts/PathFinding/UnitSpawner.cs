using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField] private GruntEnemyUnit _gruntEnemyUnit;
    [SerializeField] private int _gruntSpawnCount;

    [SerializeField] private RangedEnemyUnit _rangedEnemyUnit;
    [SerializeField] private int _rangedUnitSpawnCount;
    
    private void Start()
    {
        for (var i = 0; i < _gruntSpawnCount; i++)
        {
            var gruntUnit = _gruntEnemyUnit.Get<GruntEnemyUnit>(null, GetRandomSpawnPosition(), Quaternion.identity);
            gruntUnit.Register();
        }

        for (var i = 0; i < _rangedUnitSpawnCount; i++)
        {
            var rangedUnit = _rangedEnemyUnit.Get<RangedEnemyUnit>(null, GetRandomSpawnPosition(), Quaternion.identity);
            rangedUnit.Register();
        }
    }

    private Vector2 GetRandomSpawnPosition()
    {
        var screenSize = new Vector2(Camera.main.orthographicSize * Camera.main.aspect, Camera.main.orthographicSize);
        
        return new Vector2(Random.Range(-screenSize.x, screenSize.x), Random.Range(-screenSize.y, screenSize.y));
    }
}