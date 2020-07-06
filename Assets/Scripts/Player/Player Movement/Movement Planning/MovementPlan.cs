using System.Collections.Generic;
using UnityEngine;

public class MovementPlan
{
    public readonly Queue<IUnit> CurrentIntersections;
    public readonly ObstaclePathFinder ObstaclePathFinder;
    public bool HeadingForIntersection { get; }
    public bool Finished { get; }
    public Vector2 TargetLocation { get; private set; }
    public Vector2 MoveDirection { get; }
    public IUnit TargetUnit { get; }
    public bool IsFirst { get; private set; }
    public bool HeadingForObstacle { get; }

    public MovementPlan(MovementPlanValues movementPlanValues, MovementPlanStates movementPlanStates)
    {
        TargetLocation = movementPlanValues.TargetLocation;
        MoveDirection = movementPlanValues.MoveDirection;
        TargetUnit = movementPlanValues.TargetUnit;
        CurrentIntersections = movementPlanValues.Intersections;
        ObstaclePathFinder = movementPlanValues.ObstaclePathFinder;
        HeadingForObstacle = movementPlanStates.HeadingForObstacle;
        HeadingForIntersection = movementPlanStates.HeadingForIntersection;
        Finished = movementPlanStates.Finished;
    }


    public bool ValidIntersections()
    {
        return CurrentIntersections != null && CurrentIntersections.Count > 0;
    }

    public void UpdateTargetLocationWhileMovingToIntersect(Vector2 newTargetLocation)
    {
        TargetLocation = newTargetLocation;
    }

    public void SetFirst()
    {
        IsFirst = true;
    }
}