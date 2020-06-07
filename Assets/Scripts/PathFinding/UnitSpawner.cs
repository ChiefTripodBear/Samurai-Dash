using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField] private GruntUnit _gruntUnit;
    [SerializeField] private int _gruntSpawnCount;

    [SerializeField] private RangedUnit _rangedUnit;
    [SerializeField] private int _rangedUnitSpawnCount;
    
    private void Start()
    {
        for (var i = 0; i < _gruntSpawnCount; i++)
        {
            var gruntUnit = _gruntUnit.Get<GruntUnit>(null, GetRandomSpawnPosition(), Quaternion.identity);
            gruntUnit.Register();
        }

        for (var i = 0; i < _rangedUnitSpawnCount; i++)
        {
            var rangedUnit = _rangedUnit.Get<RangedUnit>(null, GetRandomSpawnPosition(), Quaternion.identity);
            rangedUnit.Register();
        }
    }

    private Vector2 GetRandomSpawnPosition()
    {
        var screenSize = new Vector2(Camera.main.orthographicSize * Camera.main.aspect, Camera.main.orthographicSize);
        
        return new Vector2(Random.Range(-screenSize.x, screenSize.x), Random.Range(-screenSize.y, screenSize.y));
    }
}