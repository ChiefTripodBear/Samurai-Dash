using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private int _waveCount = 30;
    [SerializeField] private bool _looping;
    [SerializeField] private float _spawnDelay = 5f;
    private Camera _mainCam;

    private const int ScreenCushion = 2;

    private Collider2D _playerCollider;

    private IEnumerator Start()
    {
        _playerCollider = FindObjectOfType<Player>().GetComponent<CircleCollider2D>();
        _mainCam = Camera.main;
        do
        {
            yield return StartCoroutine(SpawnEnemies());
        } 
        while (_looping);
    }

    private IEnumerator SpawnEnemies()
    {
        for (var i = 0; i < _waveCount; i++)
        {
            var enemy = _enemyPrefab.Get<Enemy>(null, FindSpawnPoint(), Quaternion.identity); 
            yield return new WaitForSeconds(_spawnDelay);
        }
    }

    private Vector2 FindSpawnPoint()
    {
        var screenSize = new Vector2(_mainCam.aspect * _mainCam.orthographicSize, _mainCam.orthographicSize);

        var position = GetRandomPoint(screenSize);

        while (_playerCollider.bounds.Contains(position))
        {
            position = GetRandomPoint(screenSize);
        }

        return position;
    }

    private static Vector2 GetRandomPoint(Vector2 screenSize)
    {
        return new Vector2(
            Random.Range(-screenSize.x + ScreenCushion, screenSize.x - ScreenCushion), 
            Random.Range(-screenSize.y + ScreenCushion, screenSize.y - ScreenCushion));
    }
}


