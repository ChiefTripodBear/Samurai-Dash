﻿using UnityEngine;

public class ParallelAnalysis
{
    public IUnit ParallelUnit;
    public bool FoundInvalidEnemy;

    public ParallelAnalysis(Destination destination, float distanceScalar)
    {
        CheckForParallelEnemiesAtDestination(destination, distanceScalar);
    }
    
    private void CheckForParallelEnemiesAtDestination(Destination destination, float distanceScalar)
    {
        distanceScalar = destination.PreviousIntersectingUnit != null
            ? Vector2.Distance(destination.PreviousIntersectingUnit.Transform.position, destination.PreviousIntersectingUnit.AngleDefinition.IntersectionPoint)
            : distanceScalar;
        var rayFirePosition = GetFirePositionFromDestinationType(destination);

        if (rayFirePosition.HasValue)
        {
            // FoundInvalidEnemy =
            //     TargetDetector.FoundInvalidEnemy(distanceScalar, destination.MoveDirection, rayFirePosition.Value);
            // ParallelUnit = TargetDetector.GetValidUnitInFrontFromTargetPosition(destination.Unit, distanceScalar,
            //     destination.MoveDirection, rayFirePosition.Value);
        }
    }

    private Vector2? GetFirePositionFromDestinationType(Destination destination)
    {
        switch (destination.DestinationType)
        {
            case DestinationType.ClosestPointToEnemy : return destination.Unit.KillHandler.KillPoint.Value;
            case DestinationType.Intersection : return destination.TargetLocation; 
            default: return null;
        }
    }
}