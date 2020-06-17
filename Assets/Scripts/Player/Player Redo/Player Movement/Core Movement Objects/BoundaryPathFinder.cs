using System.Collections.Generic;
using UnityEngine;

public class BoundaryPathFinder
{
    private readonly Transform _mover;
    private Destination _destination;

    public List<Vector2> BoundaryPath;
    public DestinationType PreviousDestinationType { get; }

    public BoundaryPathFinder(Transform mover, Destination destination)
    {
        _mover = mover;
        _destination = destination;
        PreviousDestinationType = destination.DestinationType;
        EvaluateBoundaries();
    }


    private void EvaluateBoundaries()
    {
        if (BoundaryHelper.WillBeMovingThroughBoundary(_mover.position, _destination.TargetLocation, out var boundary))
        {
            if (boundary == null) return;
            
            _destination.DestinationType = DestinationType.BoundaryPath;
            BoundaryPath = boundary.BoundaryPath(_destination.TargetLocation, _mover.transform.position);
        }
    }
}