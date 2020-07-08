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
        var targetLocation = unit?.AngleDefinition.GetPointClosestTo(_mover.position) ?? StartingTargetLocation(_mover.position, moveDirection);
        
        targetLocation =
            BoundaryHelper.HandleBoundaryCollision(targetLocation, moveDirection);

        // Obstacle start position = mover.position
        // Target end position = targetLocation (only adjust if there is no unit)

        _currentIntersections = _intersectionDetector.GetIntersectionsFromUnit(unit);
        
        var startingPlanValues = new MovementPlanValues(targetLocation, moveDirection, unit, _currentIntersections);
        var startingPlanStates = new MovementPlanStates(false, unit == null, false);

        var movementPlan = new MovementPlan(startingPlanValues, startingPlanStates);
        
        movementPlan.SetFirst();
        
        OnFirstMove?.Invoke(1);

        return EditMovementPlanForObstacles(movementPlan, _mover.position);
    }

    public MovementPlan PlanMovingTowardsIntersectingUnitFromPreviousIntersection(MovementPlan previousPlan)
    {
        Debug.Log(_mover.transform);
        var targetLocation = previousPlan.TargetUnit.AngleDefinition.GetPointClosestTo(_mover.position);

        var moveDirection = (targetLocation - previousPlan.TargetLocation).normalized;

        _currentIntersections = _intersectionDetector.GetIntersectionsFromUnit(previousPlan.TargetUnit);
        
        // Obstacle start position = previousPlan.TargetLocation
        // Target end position = targetLocation (never adjusted)
        var successfulRedirectionValues = new MovementPlanValues(targetLocation, moveDirection, previousPlan.TargetUnit, _currentIntersections);
        var successfulRedirectStates = new MovementPlanStates(false, false, false);

        var movementPlan = new MovementPlan(successfulRedirectionValues, successfulRedirectStates);

        return EditMovementPlanForObstacles(movementPlan, previousPlan.TargetLocation);
    }
    
    public MovementPlan FinishPlanOrMoveToNextIntersect(MovementPlan previousPlan)
    {
        MovementPlan movementPlan;
        if (previousPlan.ValidIntersections())
        {
            _currentIntersections = previousPlan.CurrentIntersections;
            var targetUnit = _currentIntersections.Dequeue();
            var targetLocation = targetUnit.AngleDefinition.IntersectionPoint;

            // Obstacle start position = previousPlan.TargetLocation
            // Target end position = targetLocation (never adjusted)

            var nextIntersectionMovementValues = new MovementPlanValues(targetLocation, previousPlan.MoveDirection, targetUnit, _currentIntersections);
            var nextIntersectionMovementStates = new MovementPlanStates(true, false, false);

            movementPlan = new MovementPlan(nextIntersectionMovementValues, nextIntersectionMovementStates);

            return EditMovementPlanForObstacles(movementPlan, previousPlan.TargetLocation);
        }
        
        var finishedTargetPosition = FinishedTargetLocation(previousPlan.TargetLocation, previousPlan.MoveDirection);
        
        finishedTargetPosition =
            BoundaryHelper.HandleBoundaryCollision(finishedTargetPosition, previousPlan.MoveDirection);

        // Obstacle start position = previousPlan.TargetLocation
        // Target end position = Adjusted(finishedTargetPosition);
        
        var finishedMovementValues = new MovementPlanValues(finishedTargetPosition, previousPlan.MoveDirection, null, null);
        var finishedMovementStates = new MovementPlanStates(false, true, false);
        
        movementPlan = new MovementPlan(finishedMovementValues, finishedMovementStates);

        return EditMovementPlanForObstacles(movementPlan, previousPlan.TargetLocation);
    }

    private MovementPlan EditMovementPlanForObstacles(MovementPlan movementPlan, Vector2 fromPosition)
    {
        var obstaclePriority = _obstaclePathFinder.HasObstaclePath();

        if (!obstaclePriority)
        {
            obstaclePriority =
                _obstaclePathFinder.FoundPath(fromPosition, movementPlan.TargetLocation, movementPlan.MoveDirection);
        }

        if (!obstaclePriority)
        {
            return movementPlan;
        }
        var targetLocation = _obstaclePathFinder.NextPoint().ProjectedPoint;
        var targetMoveDirection = (targetLocation - fromPosition).normalized;
        var updatedPlanValues = new MovementPlanValues(targetLocation, targetMoveDirection, movementPlan.TargetUnit, _currentIntersections);
        var updatedPlanStates = new MovementPlanStates(movementPlan.HeadingForIntersection, movementPlan.Finished, true);
        
        return new MovementPlan(updatedPlanValues, updatedPlanStates);
    }

    public MovementPlan GetPlanForNextMoveDuringObstaclePath(MovementPlan previousPlan)
    {
        var targetLocation = !_obstaclePathFinder.HasObstaclePath()
            ? _obstaclePathFinder.TargetLocationOnceFinishedWithObstaclePath
            : _obstaclePathFinder.NextPoint().ProjectedPoint;

        var moveDirection = !_obstaclePathFinder.HasObstaclePath()
            ? (_obstaclePathFinder.TargetLocationOnceFinishedWithObstaclePath -
               _obstaclePathFinder.PointBeforeStartingObstaclePath).normalized
            : (targetLocation - previousPlan.TargetLocation).normalized;
        
        var obstaclePlanValues = new MovementPlanValues(targetLocation, moveDirection, previousPlan.TargetUnit, previousPlan.CurrentIntersections);
        var obstaclePlanStates = new MovementPlanStates(previousPlan.HeadingForIntersection, previousPlan.TargetUnit == null, _obstaclePathFinder.HasObstaclePath());
        
        return new MovementPlan(obstaclePlanValues, obstaclePlanStates);
    }
    
    private Vector2 StartingTargetLocation(Vector2 fromLocation, Vector2 moveDirection)
    {
        return fromLocation + moveDirection * _distanceScalar;
    }
    
    private Vector2 FinishedTargetLocation(Vector2 fromPosition, Vector2 moveDirection)
    {
        return fromPosition + moveDirection * (_distanceScalar + _distanceScalar * 0.5f);
    }
}

// Get Point to check from
// Get Initial Target Location
// check for obstacles
// if there is an obstacle - adjust target location
// store adjusted target location
// store initial check from
// plot path around obstacle
// move through path
// once done with path, set targetlocation to adjusted target location