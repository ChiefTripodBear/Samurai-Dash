using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitChainEvaluator 
{
    private static List<IUnit> _units = new List<IUnit>();
    
    public static Queue<IUnit> GetIntersectionsRelativeTo(IUnit firstUnit)
    {
        var possibleIntersections = new List<IUnit>();
        var firstStartCheckPoint = firstUnit.Transform.position;

        foreach (var unit in _units.ToList())
        {    
            if(unit == firstUnit 
               || unit == null 
               || unit.Transform.gameObject.activeInHierarchy == false 
               || unit.KillHandler.KillPoint.HasValue 
               || unit.AngleDefinition.IntersectionPoint != Vector2.zero && BoundaryHelper.ContainedInObstacleCollider(unit.AngleDefinition.IntersectionPoint)) continue;

            var firstRearCheckPoint = firstUnit.AngleDefinition.RearPointRelative;
            
            var rearPoint = unit.AngleDefinition.RearPointRelative;
            var forwardPoint = unit.AngleDefinition.ForwardPointRelative;
            
            if(IntersectionMaths.IsIntersecting(firstStartCheckPoint, firstRearCheckPoint, rearPoint, forwardPoint))
            {
                var intersect = IntersectionMaths.FindIntersection(firstStartCheckPoint, firstRearCheckPoint,
                    rearPoint, forwardPoint);

                if (intersect != null)
                {
                    var directionThroughKillPoint =
                        (unit.KillHandler.GetFauxKillPoint() - (Vector2) unit.Transform.position).normalized;
                    if(possibleIntersections.Any(t => Vector2.Distance(intersect.Value, t.AngleDefinition.IntersectionPoint) < 1f)) continue;
                    if (!NodeGrid.Instance.NodeFromWorldPosition(intersect.Value).IsWalkable 
                        || BoundaryHelper.WillCollideWithBoundaryAtTargetLocation(unit.KillHandler.GetFauxKillPoint(), directionThroughKillPoint, 1.5f)
                        || !BoundaryHelper.OnScreen(intersect.Value)
                        || BoundaryHelper.ContainedInObstacleCollider(intersect.Value)) continue;

                    if(Vector2.Distance(unit.Transform.position, intersect.Value) < 2f 
                       || Vector2.Distance(unit.Transform.position, firstStartCheckPoint) < 1f
                       || Vector2.Distance(firstUnit.Transform.position, intersect.Value) < 1f) continue;
                    
                    unit.AngleDefinition.SetIntersectionPoint(intersect.Value);
                    possibleIntersections.Add(unit);
                }
            }
        }

        if (possibleIntersections.Count <= 0) return null;
        
        var orderedIntersections = possibleIntersections.OrderBy(t =>
            Vector2.Distance(firstStartCheckPoint, t.AngleDefinition.IntersectionPoint));
        
        var intersectOrder = new Queue<IUnit>();

        for (var i = 0; i < orderedIntersections.Count(); i++)
        {
            intersectOrder.Enqueue(orderedIntersections.ElementAt(i));
        }
        return intersectOrder.Count > 0 ? intersectOrder : null;
    }
    
    public static void Clear()
    {
        _units.Clear();
    }

    public static void AddUnit(IUnit unit)
    {
        if (_units.Contains(unit)) return;
        _units.Add(unit);
    }

    public static void RemoveUnit(IUnit unit)
    {
        _units.Remove(unit);
    }
}