using UnityEngine;

public class SpawnHelper : MonoBehaviour
{
    private static SpawnHelper _instance;
    public static SpawnHelper Instance => _instance;
    
    private NodeGrid _grid;
    private Player _player;
    private Camera _mainCam;
    private void Awake()
    {
        if (_instance == null) _instance = this;

        _player = FindObjectOfType<Player>();
        _grid = FindObjectOfType<NodeGrid>();
        _mainCam = Camera.main;
    }

    public Vector2 ValidSpawnPosition()
    {
        Vector2? bestPosition = null;

        while (bestPosition == null) bestPosition = FindBestNode();

        return bestPosition.Value;
    }

    private Vector2? FindBestNode()
    {
        var position = GetRandomSpawnPosition();
        var invalidPosition = IsInvalid(position);
        return invalidPosition ? null : (Vector2?)position;
    }

    private bool IsInvalid(Vector2 position)
    {
        return _grid.WorldPositionOnUnwalkableLayer(position) || _grid.WorldPositionIsOnNodeInSafetyGrid(_player.transform.position, position);
    }

    private Vector2 GetRandomSpawnPosition()
    {
        var screenSize = new Vector2(_mainCam.orthographicSize * _mainCam.aspect, _mainCam.orthographicSize);
        
        return new Vector2(Random.Range(-screenSize.x, screenSize.x), Random.Range(-screenSize.y, screenSize.y));
    }
}