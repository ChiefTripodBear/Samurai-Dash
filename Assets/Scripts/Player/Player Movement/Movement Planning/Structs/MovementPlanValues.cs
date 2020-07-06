using System.Collections.Generic;
using UnityEngine;

public readonly struct MovementPlanValues
{
    public Vector2 TargetLocation { get; }
    public Vector2 MoveDirection { get; }
    public IUnit TargetUnit { get; }
    public Queue<IUnit> Intersections { get; }
    public ObstaclePathFinder ObstaclePathFinder { get; }

    public MovementPlanValues(Vector2 targetLocation, Vector2 moveDirection, IUnit targetUnit, Queue<IUnit> intersections, ObstaclePathFinder obstaclePathFinder)
    {
        TargetLocation = targetLocation;
        MoveDirection = moveDirection;
        TargetUnit = targetUnit;
        Intersections = intersections;
        ObstaclePathFinder = obstaclePathFinder;
    }
}