using System;
using UnityEngine;

public class UnitWayPointMover : IPathRequester
{
    public event Action<PathRequest> PathRequested;
    public event Action PathCompleted;

    private readonly IUnitEnemy _enemyUnit;
    public Transform Mover { get; }

    private RingPosition _currentWayPoint;
    private int _pathRequestAttemptsBeforeRandomizingLocation = 5;
    private int _randomizationAttempts = 5;
    private PathValues _ringPathValues;
    private PathValues _spawnPathValues;

    public UnitWayPointMover(IUnitEnemy enemyUnit)
    {
        _enemyUnit = enemyUnit;
        Mover = enemyUnit.Transform;
        
        _enemyUnit.KillHandler.OnDeath += () =>
        {
            enemyUnit.UnitManager.TurnInRingPosition(_currentWayPoint as RingPosition);
        };
        
        _ringPathValues = new PathValues(4, .1f, 1f, PathType.Ring);
        _spawnPathValues = new PathValues(3.5f, 0f, 0f, PathType.Spawn);
    }
        
    private void SetWayPoint(RingPosition wayPoint)
    {
        if (wayPoint == null) return;
        
        _currentWayPoint = wayPoint;
        _currentWayPoint.Claim(_enemyUnit);
    }

    public void RequestRingPath()
    {
        PathRequested?.Invoke(new PathRequest(this, _ringPathValues, _currentWayPoint, FindPath, OnPathComplete));
    }

    private Vector2[] FindPath()
    {
        SetWayPoint(_enemyUnit.UnitManager.GetNextAvailableRingPosition(_currentWayPoint));

        Vector2[] path = null;

        if (_currentWayPoint != null)
        {
            for (var i = 0; i < _pathRequestAttemptsBeforeRandomizingLocation; i++)
            {
                path = Pathfinder.Path(_enemyUnit, _currentWayPoint.transform.position);
                
                if (path != null && path.Length > 0) break;

                SetWayPoint(_enemyUnit.UnitManager.GetNextAvailableRingPosition(_currentWayPoint));
            }
        }
        
        if (path == null)
            for (var i = 0; i < _randomizationAttempts; i++)
            {
                var randomPosition = SpawnHelper.Instance.ValidPointOnScreen();
                path = Pathfinder.Path(_enemyUnit, randomPosition);

                if (path != null && path.Length > 0) break;
            }
        
        
        if (path == null)
        {
            Debug.LogError($"No path found for {Mover.transform}!");
            return null;
        }

        return path;
    }

    public void RequestSpawnPath(Vector2 destination)
    {
        PathRequested?.Invoke(new PathRequest(this, _spawnPathValues, () => (Vector2?) destination, OnPathComplete));
    }

    private void OnPathComplete(bool status)
    {
        PathCompleted?.Invoke();
    }
}