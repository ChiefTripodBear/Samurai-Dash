using UnityEngine;

public class UnitMovementManager
{
    private int _pathRequestAttemptsBeforeRandomizingLocation = 5;
    
    private readonly Transform _mover;
    private readonly EnemyUnitMover _unitEnemyMover;
    private Pathfinder _pathfinder;
    private UnitManager _unitManager;

    private IWaypoint _currentWayPoint;

    private bool _completedRequest;
    private Coroutine _moveRoutine;

    public UnitMovementManager(Transform mover, IUnitEnemy unitEnemy)
    {
        _pathfinder = new Pathfinder(unitEnemy, Object.FindObjectOfType<Player>(), Object.FindObjectOfType<NodeGrid>());
        _mover = mover;
        _unitManager = unitEnemy.UnitManager;
        _unitEnemyMover = unitEnemy.EnemyUnitMover;
        _unitEnemyMover.NeedPath += GetPath;

        unitEnemy.KillHandler.OnDeath += () =>
        {
            _unitEnemyMover.CanMoveThroughPath = false;
            _unitManager.TurnInRingPosition(_currentWayPoint as RingPosition);
        };
        
        unitEnemy.OnActivated += () =>
        {
            _unitEnemyMover.CanMoveThroughPath = true;
            GetPath();
        };    
    }

    private void GetPath()
    {
        SetWayPoint(_unitManager.GetNextAvailableRingPosition(_currentWayPoint));

        Vector2[] path = null;

        if (_currentWayPoint == null) return;

        
        for (var i = 0; i < _pathRequestAttemptsBeforeRandomizingLocation; i++)
        {
            path = _pathfinder.Path(_currentWayPoint.Transform.position);

            if (path != null && path.Length > 0) break;

            SetWayPoint(_unitManager.GetNextAvailableRingPosition(_currentWayPoint));
        }

        if (path == null)
            for (var i = 0; i < _pathRequestAttemptsBeforeRandomizingLocation; i++)
            {
                var randomPosition = SpawnHelper.Instance.ValidSpawnPosition();
                path = _pathfinder.Path(randomPosition);

                if (path != null && path.Length > 0) break;
            }

        if (path == null)
        {
            Debug.LogError($"No path found for {_mover.transform}!");
            return;
        }

        _moveRoutine = _unitManager.StartCoroutine(_unitEnemyMover.MoveToWayPoint(path, _currentWayPoint, _mover, 4f));
    }
    

    private void SetWayPoint(IWaypoint wayPoint)
    {
        _currentWayPoint = wayPoint;
        _currentWayPoint.Claim(_unitEnemyMover);
    }

    public void MoveForTime(Vector3 targetPosition, Transform mover, float speed, float time)
    {
        var path = _pathfinder.Path(targetPosition);

        if (_moveRoutine != null)
            _unitManager.StopCoroutine(_moveRoutine);
        
        _moveRoutine = _unitManager.StartCoroutine(_unitEnemyMover.MoveThroughPathForTime(path, mover, speed, time));
    }
}