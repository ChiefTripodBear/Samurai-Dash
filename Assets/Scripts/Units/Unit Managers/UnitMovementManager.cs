using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

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
    private IUnitEnemy _unitEnemy;

    public UnitMovementManager(Transform mover, IUnitEnemy unitEnemy)
    {
        _pathfinder = new Pathfinder(unitEnemy, Object.FindObjectOfType<Player>(), Object.FindObjectOfType<NodeGrid>());
        _mover = mover;
        _unitEnemy = unitEnemy;
        _unitManager = unitEnemy.UnitManager;
        _unitEnemyMover = unitEnemy.EnemyUnitMover;
        _unitEnemyMover.NeedPath += GetPath;

        unitEnemy.KillHandler.OnDeath += () =>
        {
            Debug.Log("Died - movement manager");
            _unitEnemyMover.CanMoveThroughPath = false;
            _unitManager.TurnInRingPosition(_currentWayPoint as RingPosition);
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
                var randomPosition = SpawnHelper.Instance.ValidPointOnScreen();
                path = _pathfinder.Path(randomPosition);

                if (path != null && path.Length > 0) break;
            }

        if (path == null)
        {
            Debug.LogError($"No path found for {_mover.transform}!");
            return;
        }
        
        if(_unitEnemy.Transform.gameObject.activeInHierarchy)
            _moveRoutine = _unitEnemy.MonoBehaviour.StartCoroutine(_unitEnemyMover.MoveToWayPoint(path, _currentWayPoint, _mover, 4f));
    }

    public bool RequestPathGivenDestination(Vector2 destination, Action pathArrivalCallback)
    {
        var path = _pathfinder.Path(destination);

        if (path == null || path.Length <= 0) return false;

        _unitEnemyMover.CanMoveThroughPath = true;

        _moveRoutine = _unitEnemy.MonoBehaviour.StartCoroutine( _unitEnemyMover.MoveThroughPathWithNoWayPoint(path, _mover, 4f, 0f, pathArrivalCallback));
        return true;
    }

    public void MoveToPoint(Vector2 point, bool priority, float endWaitTime, float startWaitTime)
    {
        if (priority)
        {
            _unitEnemy.MonoBehaviour.StopAllCoroutines();
        }
        _unitEnemyMover.CanMoveThroughPath = true;
        _moveRoutine = _unitEnemy.MonoBehaviour.StartCoroutine(_unitEnemyMover.MoveToPointWithoutPathfinder(_mover, point, 4f, endWaitTime, startWaitTime));
    }

    public void MoveToPoint(Vector2 point, bool priority, float endWaitTime, float startWaitTime, float moveSpeed,
        Action pathArrivalCallback)
    {
        if (priority)
        {
            _unitEnemy.MonoBehaviour.StopAllCoroutines();
        }
        _unitEnemyMover.CanMoveThroughPath = true;
        _moveRoutine = _unitEnemy.MonoBehaviour.StartCoroutine(_unitEnemyMover.MoveToPointWithoutPathfinder(_mover, point, moveSpeed, endWaitTime, startWaitTime, pathArrivalCallback));
    }
    
    private void SetWayPoint(IWaypoint wayPoint)
    {
        _currentWayPoint = wayPoint;
        _currentWayPoint.Claim(_unitEnemyMover);
    }

    public void MoveForTime(Vector3 targetPosition, Transform mover, float speed, float time)
    {
        var path = _pathfinder.Path(targetPosition);
        
        _unitEnemy.MonoBehaviour.StartCoroutine(_unitEnemyMover.MoveThroughPathForTime(path, mover, speed, time));
    }
}