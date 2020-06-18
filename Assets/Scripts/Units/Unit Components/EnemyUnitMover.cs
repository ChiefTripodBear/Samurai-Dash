using System;
using System.Collections;
using UnityEngine;

public abstract class EnemyUnitMover
{
    public abstract event Action InheritancePathRequest;
    public event Action NeedPath;
    private int _pathIndex;
    private Vector2 _startingDestinationPosition;
    public bool CanMoveThroughPath { get; set; } = true;
    public Vector2? CurrentDestination { get; private set; }
    public bool IsCurrentlyMoving { get; private set; }

    protected EnemyUnitMover()
    {
        InheritancePathRequest += () => NeedPath?.Invoke();
    }

    public IEnumerator MoveToWayPoint(Vector2[] path, IWaypoint targetWayPoint, Transform mover, float moveSpeed)
    {
        _startingDestinationPosition = targetWayPoint.Transform.position;

        if (path == null || path.Length <= 0)
        {
            CurrentDestination = null;
            _pathIndex = 0;
            NeedPath?.Invoke();
            yield break;
        }

        while (true)
        {
            if (!CanMoveThroughPath)
            {
                IsCurrentlyMoving = false;
                yield break;
            }

            IsCurrentlyMoving = true;

            if (FinishedPath(path.Length))
            {
                yield return new WaitForSeconds(1f);
                IsCurrentlyMoving = false;
                CurrentDestination = null;
                _pathIndex = 0;
                NeedPath?.Invoke();
                yield break;
            }
            
            var currentWayPoint = path[_pathIndex];
            
            CurrentDestination = currentWayPoint;

            if (ArrivedAtPoint(mover, currentWayPoint))
            {
                _pathIndex++;

                if (TargetMoved(_startingDestinationPosition, targetWayPoint.Transform.position))
                {
                    _pathIndex = 0;
                    
                    yield return new WaitForSeconds(.5f);
                    IsCurrentlyMoving = false;
                    CurrentDestination = null;
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

    public IEnumerator MoveThroughPathWithNoWayPoint(Vector2[] path, Transform mover, float moveSpeed,
        float waitTimeAtPathEnd, Action pathArrivalCallback)
    {
        if (path == null || path.Length <= 0)
        {
            IsCurrentlyMoving = false;
            CurrentDestination = null;
            _pathIndex = 0;
            NeedPath?.Invoke();
            yield break;
        }

        while (true)
        {
            if (!CanMoveThroughPath)
            {
                IsCurrentlyMoving = false;
                yield break;
            }

            IsCurrentlyMoving = true;

            if (FinishedPath(path.Length))
            {
                yield return new WaitForSeconds(waitTimeAtPathEnd);
                IsCurrentlyMoving = false;
                CurrentDestination = null;
                _pathIndex = 0;
                pathArrivalCallback();
                yield break;
            }
            
            var currentWayPoint = path[_pathIndex];
            
            CurrentDestination = currentWayPoint;

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

    public IEnumerator MoveThroughPathWithNoWayPoint(Vector2[] path, Transform mover, float moveSpeed,
        float waitTimeAtPathEnd)
    {
        if (path == null || path.Length <= 0)
        {
            IsCurrentlyMoving = false;
            CurrentDestination = null;
            _pathIndex = 0;
            NeedPath?.Invoke();
            yield break;
        }

        while (true)
        {
            if (!CanMoveThroughPath)
            {
                IsCurrentlyMoving = false;
                yield break;
            }

            IsCurrentlyMoving = true;

            if (FinishedPath(path.Length))
            {
                yield return new WaitForSeconds(waitTimeAtPathEnd);
                IsCurrentlyMoving = false;
                CurrentDestination = null;
                _pathIndex = 0;
                yield break;
            }

            var currentWayPoint = path[_pathIndex];

            CurrentDestination = currentWayPoint;

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

    public IEnumerator MoveThroughPathForTime(Vector2[] path, Transform mover, float moveSpeed, float seconds)
    {
        if (path == null || path.Length <= 0)
        {
            yield return new WaitForSeconds(seconds);
            _pathIndex = 0;
            CurrentDestination = null;
            NeedPath?.Invoke();
            yield break;
        }
    
        yield return new WaitForSeconds(.2f);

        for (var i = 0f; i < seconds; i += Time.deltaTime)
        {
            IsCurrentlyMoving = true;

            if (!CanMoveThroughPath)
            {
                IsCurrentlyMoving = false;
                yield break;
            }
        
            if (FinishedPath(path.Length))
            {
                yield return new WaitForSeconds(.5f);
                IsCurrentlyMoving = false;
                _pathIndex = 0;
                CurrentDestination = null;
                NeedPath?.Invoke();
                yield break;
            }

            var currentWayPoint = path[_pathIndex];

            CurrentDestination = currentWayPoint;

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

    public IEnumerator MoveToPointWithoutPathfinder(Transform mover, Vector2 point, float moveSpeed, float endWaitTime, float startWaitTime)
    {
        yield return new WaitForSeconds(startWaitTime);
        
        while (!ArrivedAtPoint(mover, point))
        {
            IsCurrentlyMoving = true;
            
            if (!CanMoveThroughPath)
            {
                IsCurrentlyMoving = false;
                NeedPath?.Invoke();
                yield break;
            }
        
            Move(mover, moveSpeed, point);
            yield return null;
        }

        yield return new WaitForSeconds(endWaitTime);
        NeedPath?.Invoke();
    }
    
    public IEnumerator MoveToPointWithoutPathfinder(Transform mover, Vector2 point, float moveSpeed, float endWaitTime, float startWaitTime, Action pathArrivalCallback)
    {
        yield return new WaitForSeconds(startWaitTime);
        
        while (!ArrivedAtPoint(mover, point))
        {
            IsCurrentlyMoving = true;
            
            if (!CanMoveThroughPath)
            {
                pathArrivalCallback();
                IsCurrentlyMoving = false;
                yield break;
            }
        
            Move(mover, moveSpeed, point);
            yield return null;
        }

        yield return new WaitForSeconds(endWaitTime);
        pathArrivalCallback();
    }

    private bool FinishedPath(int pathCount) => _pathIndex >= pathCount;

    private static bool ArrivedAtPoint(Transform mover, Vector2 currentWayPoint) 
        => Vector2.Distance(mover.transform.position, currentWayPoint) < 0.1f;

    private static void Move(Transform mover, float moveSpeed, Vector2 currentWayPoint) 
        => mover.position = Vector2.MoveTowards(mover.position, currentWayPoint, moveSpeed * Time.deltaTime);

    private static bool TargetMoved(Vector2 startingPosition, Vector2 currentDestination) 
        => Vector2.Distance(startingPosition, currentDestination) > 2f;

    public void DrawGizmos()
    {
        if (CurrentDestination.HasValue)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(CurrentDestination.Value, 1f);
        }
    }
}
