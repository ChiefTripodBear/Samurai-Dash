using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IntersectionAnalysis
{
    public Queue<IUnit> AllIntersections;
    public IUnit IntersectingUnit;
    public IntersectionAnalysis(Destination destination)
    {
        if (destination.Unit == null) return;

        AllIntersections = UnitChainEvaluator.Instance.GetIntersectionsRelativeTo(destination.Unit);
        
        if (AllIntersections != null)
        {
            var intersections = AllIntersections.ToList();
            for (var i = 0; i < intersections.Count; i++)
            {
                if (i == 0)
                {
                    RedirectDisplayManager.Instance.SetActiveDisplayWithIntersectionAt(destination.TargetLocation);
                }

                var direction = intersections[i].AngleDefinition.IntersectionPoint - (Vector2)intersections[i].Transform.position;
                RedirectDisplayManager.Instance.DisplayCorrectVector(direction, intersections[i].AngleDefinition.IntersectionPoint);
            }
        }
    }
    public bool HasIntersections()
    {
        return AllIntersections != null && AllIntersections.Count > 0;
    }

    public IUnit PeekIntersections()
    {
        return HasIntersections() ? AllIntersections.Peek() : null;
    }

    public IUnit GetNextUnit()
    {
        IntersectingUnit = HasIntersections() ? AllIntersections.Dequeue() : null;
        return IntersectingUnit;
    }
}