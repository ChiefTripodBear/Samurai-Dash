using UnityEngine;

public class MovementPackage
{
    public static int MovementCount;
    public bool Finished { get; private set; }
    public Destination Destination { get; }
    public IntersectionAnalysis IntersectionAnalysis { get; }
    public float DistanceScalar { get; }

    private ParallelAnalysis _parallelAnalysis;
    private Transform _mover;

    public MovementPackage(Destination destination, float distanceScalar)
    {
        MovementCount++;
        DistanceScalar = distanceScalar;
        Destination = destination;
    }
    
    public MovementPackage(Destination destination, IntersectionAnalysis previousIntersectionAnalysis, Transform mover, float distanceScalar)
    {
        MovementCount++;
        DistanceScalar = distanceScalar;
        _mover = mover;
        
        Destination = destination;
        _parallelAnalysis = new ParallelAnalysis(Destination, distanceScalar);
        
        IntersectionAnalysis = ShouldUsePreviousIntersections(previousIntersectionAnalysis) 
            ? previousIntersectionAnalysis 
            : new IntersectionAnalysis(destination);

        Evaluate(distanceScalar);
    }

    private bool ShouldUsePreviousIntersections(IntersectionAnalysis previousIntersectionAnalysis)
    {
        return previousIntersectionAnalysis != null && previousIntersectionAnalysis.HasIntersections();
    }

    private void Evaluate(float distanceScalar)
    {
        if (OnlyParallelUnitFound())
        {
            SetDestinationForParallelUnit();
            return;
        }

        if (OnlyIntersectionsFound())
        {
            SetDestinationForNextIntersection();
            return;
        }

        if (ParallelAndIntersectionsFound())
        {
            var intersectionLocation = IntersectionAnalysis.PeekIntersections().AngleDefinition.IntersectionPoint;
            var parallelUnitLocation = _parallelAnalysis.ParallelUnit.Transform.position;

            var parallelIsCloser = Vector2.Distance(parallelUnitLocation, _mover.transform.position) <
                                   Vector2.Distance(intersectionLocation, _mover.transform.position);

            if (parallelIsCloser)
                SetDestinationForParallelUnit();
            else
                SetDestinationForNextIntersection();

            return;
        }

        if (NothingFound())
        {
            Finished = true;
            Destination.TargetLocation += Destination.MoveDirection * distanceScalar;
        }
    }

    private void SetDestinationForNextIntersection()
    {
        Destination.DestinationType = DestinationType.Intersection;
        Destination.Unit = null;

        Destination.TargetLocation = IntersectionAnalysis.GetNextUnit().AngleDefinition.IntersectionPoint;
        // Set move direction AFTER the redirect to determine which way we're actually going to move!
    }

    private void SetDestinationForParallelUnit()
    {
        Destination.DestinationType = DestinationType.ClosestPointToEnemy;
        Destination.Unit = _parallelAnalysis.ParallelUnit;
        Destination.Unit.KillHandler.SetKillPoint();

        Destination.TargetLocation =
            _parallelAnalysis.ParallelUnit.AngleDefinition.GetPointClosestTo(_mover.position);
        Destination.MoveDirection = ((Vector2) _parallelAnalysis.ParallelUnit.Transform.position -
                                     _parallelAnalysis.ParallelUnit.AngleDefinition.GetPointClosestTo(
                                         _mover.position)).normalized;
    }

    private bool OnlyIntersectionsFound()
    {
        return IntersectionAnalysis.HasIntersections() && _parallelAnalysis.ParallelUnit == null;
    }

    private bool OnlyParallelUnitFound()
    {
        return IntersectionAnalysis.HasIntersections() == false && _parallelAnalysis.ParallelUnit != null;
    }

    private bool ParallelAndIntersectionsFound()
    {
        return IntersectionAnalysis.HasIntersections() && _parallelAnalysis.ParallelUnit != null;
    }

    private bool NothingFound()
    {
        return IntersectionAnalysis.HasIntersections() == false && _parallelAnalysis.ParallelUnit == null;
    }
}