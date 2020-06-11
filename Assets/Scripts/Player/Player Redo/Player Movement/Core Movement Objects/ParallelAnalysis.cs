using UnityEngine;

public class ParallelAnalysis
{
    public IUnit ParallelUnit;

    public ParallelAnalysis(Destination destination, float distanceScalar)
    {
        CheckForParallelEnemiesAtDestination(destination, distanceScalar);
    }
    
    private void CheckForParallelEnemiesAtDestination(Destination destination, float distanceScalar)
    {
        distanceScalar *= 2;
        var rayFirePosition = GetFirePositionFromDestinationType(destination);

        if (rayFirePosition.HasValue)
            ParallelUnit = TargetDetector.GetUnitInFrontFromTargetPosition(destination.Unit, distanceScalar,
                destination.MoveDirection, rayFirePosition.Value);
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