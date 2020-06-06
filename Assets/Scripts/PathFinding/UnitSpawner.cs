using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField] private GruntUnit _gruntUnit;
    [SerializeField] private int _gruntSpawnCount;

    private void Awake()
    {
        for (var i = 0; i < _gruntSpawnCount; i++)
        {
            var gruntUnit = _gruntUnit.Get<GruntUnit>(null, GetRandomSpawnPosition(), Quaternion.identity);
            gruntUnit.Register();
        }
    }

    private Vector2 GetRandomSpawnPosition()
    {
        var screenSize = new Vector2(Camera.main.orthographicSize * Camera.main.aspect, Camera.main.orthographicSize);
        
        return new Vector2(Random.Range(-screenSize.x, screenSize.x), Random.Range(-screenSize.y, screenSize.y));
    }
}