using System;
using System.Collections.Generic;
using UnityEngine;

public class MovementPlanner
{
    public static event Action<int> OnFirstMove;
    private Queue<IUnit> _currentIntersections = new Queue<IUnit>();
    
    private readonly float _distanceScalar;
    private readonly IntersectionDetector _intersectionDetector;
    private readonly ObstaclePathFinder _obstaclePathFinder;
    private readonly Transform _mover;

    public MovementPlanner(Transform mover, float distanceScalar)
    {
        _mover = mover;
        _intersectionDetector = new IntersectionDetector();
        _obstaclePathFinder = new ObstaclePathFinder();
        _distanceScalar = distanceScalar;
    }
    
    public MovementPlan GetStartingPlan(Vector2 currentMoveDirection, IUnit unit)
    {
        var moveDirection = unit != null
            ? ((Vector2) unit.Transform.position - unit.AngleDefinition.GetPointClosestTo(_mover.position)).normalized
            : currentMoveDirection;
        var targetLocation = unit?.AngleDefinition.GetPointClosestTo(_mover.position) ?? NoUnitTargetLocation(_mover.position, moveDirection);
        
        _currentIntersections = _intersectionDetector.GetIntersectionsFromUnit(unit);
        
        var startingPlanValues = new MovementPlanValues(targetLocation, moveDirection, unit, _currentIntersections, null);
        var startingPlanStates = new MovementPlanStates(false, unit == null, false);

        var movementPlan = new MovementPlan(startingPlanValues, startingPlanStates);

        movementPlan.SetFirst();
        
        OnFirstMove?.Invoke(1);
        
        return movementPlan;
    }

    public MovementPlan PlanMovingTowardsIntersectingUnitFromPreviousIntersection(MovementPlan previousPlan)
    {
        var targetLocation = previousPlan.TargetUnit.AngleDefinition.GetPointClosestTo(_mover.position);
        var moveDirection = (targetLocation - previousPlan.TargetLocation).normalized;

        _currentIntersections = _intersectionDetector.GetIntersectionsFromUnit(previousPlan.TargetUnit);
        
        var successfulRedirectionValues = new MovementPlanValues(targetLocation, moveDirection, previousPlan.TargetUnit, _currentIntersections, _obstaclePathFinder);
        var successfulRedirectStates = new MovementPlanStates(false, false, false);

        return new MovementPlan(successfulRedirectionValues, successfulRedirectStates);
    }
    
    public MovementPlan FinishPlanOrMoveToNextIntersect(MovementPlan previousPlan)
    {
        if (previousPlan.ValidIntersections())
        {
            _currentIntersections = previousPlan.CurrentIntersections;
            var targetUnit = _currentIntersections.Dequeue();
            var targetLocation = targetUnit.AngleDefinition.IntersectionPoint;
            
            var nextIntersectionMovementValues = new MovementPlanValues(targetLocation, previousPlan.MoveDirection, targetUnit, _currentIntersections, _obstaclePathFinder);
            var nextIntersectionMovementStates = new MovementPlanStates(true, false, false);

            return new MovementPlan(nextIntersectionMovementValues, nextIntersectionMovementStates);
        }

        var finishedTargetPosition =
            IntersectOrUnitTargetLocation(previousPlan.TargetLocation, previousPlan.MoveDirection);
        
        var finishedMovementValues = new MovementPlanValues(finishedTargetPosition, previousPlan.MoveDirection, null, null, _obstaclePathFinder);
        var finishedMovementStates = new MovementPlanStates(false, true, false);
        
        return new MovementPlan(finishedMovementValues, finishedMovementStates);
    }

    public MovementPlan GetPlanForNextMoveDuringObstaclePath(MovementPlan previousPlan)
    {
        var targetLocation = !previousPlan.ObstaclePathFinder.HasObstaclePath()
            ? previousPlan.ObstaclePathFinder.TargetLocationOnceFinishedWithObstaclePath
            : previousPlan.ObstaclePathFinder.NextPoint().ProjectedPoint;

        var moveDirection = previousPlan.ObstaclePathFinder.HasObstaclePath() 
            ? (targetLocation - previousPlan.TargetLocation).normalized 
            : (previousPlan.ObstaclePathFinder.TargetLocationOnceFinishedWithObstaclePath - previousPlan.ObstaclePathFinder.PointBeforeStartingObstaclePath).normalized;
        
        var obstaclePlanValues = new MovementPlanValues(targetLocation, moveDirection, previousPlan.TargetUnit, previousPlan.CurrentIntersections, previousPlan.ObstaclePathFinder);
        var obstaclePlanStates = new MovementPlanStates(previousPlan.HeadingForIntersection, previousPlan.TargetUnit == null && !previousPlan.ObstaclePathFinder.HasObstaclePath(), previousPlan.ObstaclePathFinder.HasObstaclePath());

        return new MovementPlan(obstaclePlanValues, obstaclePlanStates);
    }

    private Vector2 NoUnitTargetLocation(Vector2 fromLocation, Vector2 moveDirection)
    {
        return fromLocation + moveDirection * _distanceScalar;
    }
    
    private Vector2 IntersectOrUnitTargetLocation(Vector2 fromPosition, Vector2 moveDirection)
    {
        return fromPosition + moveDirection * (_distanceScalar + _distanceScalar * 0.5f);
    }
}

public class ObstaclePathFinder
{
    private static readonly float StepTowardsObstacleSide = 0.1f;
    private Queue<ObstaclePoint> _currentObstaclePath = new Queue<ObstaclePoint>();
    public Vector2 TargetLocationOnceFinishedWithObstaclePath { get; set; }
    public ObstaclePoint PreviousObstaclePoint { get; private set; }
    public Vector2 PointBeforeStartingObstaclePath { get; set; }
    public Vector2? TargetMoveDirection { get; set; }

    public bool CheckForPath(Vector2 checkFromPosition, Vector2 checkToPosition, Vector2 moveDirection)
    {
        var obstacle = ObstacleHelper.GetObstacle(checkFromPosition, checkToPosition);
        
        if (BoundaryHelper.ContainedInObstacleCollider(checkToPosition) || BoundaryHelper.TargetLocationEndsInsideObstacle(checkToPosition))
        {
            checkToPosition = BoundaryHelper.FindPositionThroughObstacle(checkToPosition, checkFromPosition);
        }

        checkToPosition += moveDirection * 5f;
        
        if (obstacle == null)
        {
            return false;
        }
        
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
        PreviousObstaclePoint = _currentObstaclePath.Dequeue();
        return PreviousObstaclePoint;
    }
}

public static class ObstacleHelper
{
    private static List<Vector2> _adjustedPositions = new List<Vector2>();
    private static LayerMask ObstacleMask => LayerMask.GetMask("Unwalkable");

    public static ObstacleObject GetObstacle(Vector2 checkFromPosition, Vector2 checkToPosition)
    {
        var adjustedPosition = checkFromPosition;
        var distance = Vector2.Distance(adjustedPosition, checkToPosition);
        var direction = (checkToPosition - checkFromPosition).normalized;
        
        while (distance > 0.1f)
        {
            _adjustedPositions.Add(adjustedPosition);
            distance = Vector2.Distance(adjustedPosition, checkToPosition);

            var collider = Physics2D.OverlapBox(adjustedPosition, Vector2.one, 0, ObstacleMask);

            adjustedPosition += direction * 0.1f;
            
            if (collider == null)
                continue;

            var obstacleObject = collider.GetComponent<ObstacleObject>();

            if (obstacleObject != null)
                return obstacleObject;
        }
        
        return null;
    }

    public static void DrawObstacleCheckBoxes()
    {
        foreach(var adjustedPosition in _adjustedPositions)
            Gizmos.DrawWireCube(adjustedPosition, Vector2.one);
    }
}