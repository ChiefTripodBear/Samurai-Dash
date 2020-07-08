using System.Collections.Generic;
using UnityEngine;

public class ObstaclePathFinder
{
    private static readonly float StepTowardsObstacleSide = 0.01f;
    private Queue<ObstaclePoint> _currentObstaclePath = new Queue<ObstaclePoint>();
    public Vector2 TargetLocationOnceFinishedWithObstaclePath { get; set; }
    public Vector2 PointBeforeStartingObstaclePath { get; set; }

    public bool FoundPath(Vector2 checkFromPosition, Vector2 checkToPosition, Vector2 moveDirection)
    {
        var obstacle = ObstacleHelper.GetObstacle(checkFromPosition, checkToPosition);
        
        if (BoundaryHelper.ContainedInObstacleCollider(checkToPosition) || BoundaryHelper.TargetLocationEndsInsideObstacle(checkToPosition))
        {
            checkToPosition = BoundaryHelper.FindPositionThroughObstacle(checkToPosition, checkFromPosition);
        }
        
        checkToPosition += moveDirection * 5f;
        
        if (obstacle == null) return false;

        TargetLocationOnceFinishedWithObstaclePath = checkToPosition;
        PointBeforeStartingObstaclePath = checkFromPosition;

        var adjustedPointToCheckClosestToStart = checkFromPosition;

        var closestPointToStart = obstacle.FindClosestPointOnSegmentFromProjectedPoint(adjustedPointToCheckClosestToStart);

        while (closestPointToStart == null)
        {
            adjustedPointToCheckClosestToStart += moveDirection * StepTowardsObstacleSide;
            
            closestPointToStart = obstacle.FindClosestPointOnSegmentFromProjectedPoint(adjustedPointToCheckClosestToStart);
        }
        
        var adjustedPointToCheckClosestToEnd = checkToPosition;
        
        var closestPointToEnd = obstacle.FindClosestPointOnSegmentFromProjectedPoint(adjustedPointToCheckClosestToEnd);
        
        while (closestPointToEnd == null)
        {
            adjustedPointToCheckClosestToEnd -= moveDirection * StepTowardsObstacleSide;
            
            closestPointToEnd = obstacle.FindClosestPointOnSegmentFromProjectedPoint(adjustedPointToCheckClosestToEnd);
        }
        
        _currentObstaclePath = obstacle.BestPath(closestPointToStart, closestPointToEnd, checkToPosition);
        
        return _currentObstaclePath != null && _currentObstaclePath.Count > 0;
    }
    
    public bool HasObstaclePath()
    {
        return _currentObstaclePath.Count > 0;
    }

    public ObstaclePoint NextPoint()
    {
        return _currentObstaclePath.Dequeue();
    }
}