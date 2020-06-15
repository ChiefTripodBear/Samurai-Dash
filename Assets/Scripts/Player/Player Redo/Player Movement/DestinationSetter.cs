using System;
using UnityEngine;

public class DestinationSetter
{
    private Transform _transform;

    public DestinationSetter(Transform transform, Mover mover)
    {
        mover.NewMovementPackageRequested += EvaluateNextMove;
        _transform = transform;
    }

    private void EvaluateNextMove(MovementPackage previousPackage, Action<MovementPackage> callback, bool redirectSuccess)
    {
        callback(redirectSuccess
            ? ContinueFromRedirectSuccess(previousPackage)
            : ContinueFromRedirectFailure(previousPackage));

        if (redirectSuccess && previousPackage.IntersectionAnalysis.IntersectingUnit != null)
        {
            UnitChainEvaluator.Instance.RemoveUnit(previousPackage.IntersectionAnalysis.IntersectingUnit);
        }
    }

    private MovementPackage ContinueFromRedirectFailure(MovementPackage previousPackage)
    {
        return new MovementPackage(previousPackage.Destination, previousPackage.IntersectionAnalysis, _transform,
            previousPackage.DistanceScalar);
    }

    private MovementPackage ContinueFromRedirectSuccess(MovementPackage previousPackage)
    {
        var redirectedMoveDirection =
            ((Vector2) previousPackage.IntersectionAnalysis.IntersectingUnit.Transform.position - previousPackage.Destination.TargetLocation).normalized;
        
        var newRedirectDestination = new Destination();
        newRedirectDestination.Initialize(previousPackage.Destination.TargetLocation, redirectedMoveDirection, null);
        newRedirectDestination.DestinationType = previousPackage.Destination.DestinationType;
        newRedirectDestination.PreviousIntersectingUnit = previousPackage.IntersectionAnalysis.IntersectingUnit;
        return new MovementPackage(newRedirectDestination, null, _transform, previousPackage.DistanceScalar);
    }
    
    public MovementPackage GetDestinationFromFirstMove(float distanceScalar, Vector2 moveDirection)
    {
        var unit = TargetDetector.GetValidUnitInFrontFromTargetPosition(null, distanceScalar, moveDirection, _transform.position);

        var destination = EvaluateStartingMove(unit, distanceScalar, moveDirection);

        if (Vector2.Distance(_transform.position, destination.TargetLocation) < 0.1f) return null;

        var startingPackage = new MovementPackage(_transform, destination, distanceScalar);

        return startingPackage;
    }

    private Destination EvaluateStartingMove(IUnit unit, float distanceScalar, Vector2 moveDirection)
    {
        var destination = new Destination();
        var targetLocation = Vector2.zero;
        var targetMoveDirection = Vector2.zero;
        
        if (unit != null)
        {
            targetMoveDirection =
                ((Vector2) unit.Transform.position - unit.AngleDefinition.GetPointClosestTo(_transform.position))
                .normalized;
            targetLocation = unit.AngleDefinition.GetPointClosestTo(_transform.position);
            destination.DestinationType = DestinationType.ClosestPointToEnemy;
            unit.KillHandler.SetKillPoint();
            destination.Initialize(targetLocation, targetMoveDirection, unit);
        }
        else
        {
            targetMoveDirection = moveDirection;
            targetLocation = (Vector2)_transform.position + moveDirection * distanceScalar;
            destination.DestinationType = DestinationType.Exit;
            destination.Initialize(targetLocation, targetMoveDirection, null);
        }

        return destination;
    }
}