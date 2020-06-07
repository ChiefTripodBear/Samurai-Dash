using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitChainEvaluator : MonoBehaviour
{
    private static UnitChainEvaluator _instance;

    public static UnitChainEvaluator Instance => _instance;
    
    private List<IKillableWithAngle> _units = new List<IKillableWithAngle>();

    private void Start()
    {
        if (_instance == null)
            _instance = this;
    }

    public void RegisterKillableUnit(IKillableWithAngle killableUnit)
    {
        Debug.Log($"Adding {killableUnit}");
        _units.Add(killableUnit);
        killableUnit.UnitKillHandler.OnDeath += () => _units.Remove(killableUnit);
    }
    
    public Queue<IKillableWithAngle> GetIntersectionsRelativeTo(IKillableWithAngle firstUnit, Vector2 firstStartCheckPoint, Vector2 firstRearCheckPoint)
    {
        var possibleIntersections = new List<IKillableWithAngle>();

        foreach (var enemy in _units)
        {    
            if(firstUnit != null && enemy == firstUnit) continue;

            var rearPoint = enemy.UnitAngle.RearPointRelative;
            var forwardPoint = enemy.UnitAngle.ForwardPointRelative;
            if(IntersectionMaths.IsIntersecting(firstStartCheckPoint, firstRearCheckPoint, rearPoint, forwardPoint))
            {
                var intersect = IntersectionMaths.FindIntersection(firstStartCheckPoint, firstRearCheckPoint,
                    rearPoint, forwardPoint);

                if (intersect != null)
                {
                    if(Vector2.Distance(enemy.Transform.position, intersect.Value) < 2f 
                       || Vector2.Distance(enemy.Transform.position, firstStartCheckPoint) < 1f
                       || firstUnit != null && Vector2.Distance(firstUnit.Transform.position, intersect.Value) < 1f) continue;
                    
                    enemy.UnitAngle.SetIntersectionPoint(intersect.Value);
                    possibleIntersections.Add(enemy);
                }
            }
        }

        if (possibleIntersections.Count <= 0) return null;
        
        var orderedIntersections = possibleIntersections.OrderBy(t =>
            Vector2.Distance(firstStartCheckPoint, t.UnitAngle.IntersectionPoint));
        
        var intersectOrder = new Queue<IKillableWithAngle>();

        for (var i = 0; i < orderedIntersections.Count(); i++)
        {
            intersectOrder.Enqueue(orderedIntersections.ElementAt(i));
        }
        return intersectOrder.Count > 0 ? intersectOrder : null;
    }

    public void RemoveEnemyFromPotentialChains(IKillableWithAngle currentIntersectingUnit)
    {
        if(_units.Contains(currentIntersectingUnit))
            _units.Remove(currentIntersectingUnit);
    }
}