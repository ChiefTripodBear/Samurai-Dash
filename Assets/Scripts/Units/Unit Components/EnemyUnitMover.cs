﻿using System;
using System.Collections;
using UnityEngine;

public abstract class EnemyUnitMover
{
    public event Action NeedPath;
    public event Action<Vector2> AtDestination;
    public abstract event Action InheritancePathRequest;
    private int _pathIndex;
    private Vector2 _startingDestinationPosition;
    public bool CanMoveThroughPath { get; set; } = true;

    protected EnemyUnitMover()
    {
        InheritancePathRequest += () => NeedPath?.Invoke();
    }

    public IEnumerator MoveToWayPoint(Vector2[] path, IWaypoint targetWayPoint, Transform mover, float moveSpeed)
    {
        _startingDestinationPosition = targetWayPoint.Transform.position;

        if (path == null || path.Length <= 0)
        {
            _pathIndex = 0;
            NeedPath?.Invoke();
            yield break;
        }

        while (true)
        {
            if (!CanMoveThroughPath) yield break;

            if (FinishedPath(path.Length))
            {
                yield return new WaitForSeconds(1f);
                _pathIndex = 0;
                NeedPath?.Invoke();
                yield break;
            }
            
            var currentWayPoint = path[_pathIndex];
            
            if (ArrivedAtPoint(mover, currentWayPoint))
            {
                _pathIndex++;

                if (TargetMoved(_startingDestinationPosition, targetWayPoint.Transform.position))
                {
                    _pathIndex = 0;
                    AtDestination?.Invoke(currentWayPoint);
                    
                    yield return new WaitForSeconds(.5f);
                    NeedPath?.Invoke();
                    yield break;
                }
            }
            else
            {
                Move(mover, moveSpeed, currentWayPoint);
            }
            
            yield return null;
        }
    }
    
    public IEnumerator MoveThroughPathForTime(Vector2[] path, Transform mover, float moveSpeed, float seconds)
    {
        if (path == null || path.Length <= 0)
        {
            yield return new WaitForSeconds(seconds);
            _pathIndex = 0;
            NeedPath?.Invoke();
            yield break;
        }
        
        yield return new WaitForSeconds(.2f);

        for (var i = 0f; i < seconds; i += Time.deltaTime)
        {
            if (FinishedPath(path.Length))
            {
                yield return new WaitForSeconds(.5f);
                _pathIndex = 0;
                NeedPath?.Invoke();
                yield break;
            }

            var currentWayPoint = path[_pathIndex];

            if (ArrivedAtPoint(mover, currentWayPoint))
            {
                _pathIndex++;
            }
            else
            {
                Move(mover, moveSpeed, currentWayPoint);
            }
            
            yield return null;
        }
    }

    public IEnumerator MoveToPointWithoutPathfinder(Transform mover, Vector2 point, float moveSpeed)
    {
        while (!ArrivedAtPoint(mover, point))
        {
            Move(mover, moveSpeed, point);
            yield return null;
        }
        
        NeedPath?.Invoke();
    }

    private bool FinishedPath(int pathCount) => _pathIndex >= pathCount;

    private static bool ArrivedAtPoint(Transform mover, Vector2 currentWayPoint) 
        => Vector2.Distance(mover.transform.position, currentWayPoint) < 0.1f;

    private static void Move(Transform mover, float moveSpeed, Vector2 currentWayPoint) 
        => mover.position = Vector2.MoveTowards(mover.position, currentWayPoint, moveSpeed * Time.deltaTime);

    private static bool TargetMoved(Vector2 startingPosition, Vector2 currentDestination) 
        => Vector2.Distance(startingPosition, currentDestination) > 2f;
}