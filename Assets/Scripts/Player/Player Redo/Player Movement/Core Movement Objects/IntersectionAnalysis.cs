using System.Collections.Generic;
using UnityEngine;

public class IntersectionAnalysis
{
    public Queue<IUnit> AllIntersections;
    public IUnit IntersectingUnit;
    public IntersectionAnalysis(Destination destination)
    {
        if (destination.Unit == null) return;

        AllIntersections = UnitChainEvaluator.Instance.GetIntersectionsRelativeTo(destination.Unit);
    }

    public void DrawIntersectionVectors()
    {
        if (AllIntersections == null) return;
        
        var intersections = new List<IUnit>(AllIntersections);

        for (int i = 0; i < intersections.Count; i++)
        {
            var direction = intersections[i].AngleDefinition.IntersectionPoint -
                            (Vector2) intersections[i].Transform.position;

            RedirectDisplayManager.Instance.InitializeDisplayObject(direction, intersections[i]);
        }

        RedirectDisplayManager.Instance.SetActiveDisplayWithIntersectionAt(intersections[0].AngleDefinition
            .IntersectionPoint);
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