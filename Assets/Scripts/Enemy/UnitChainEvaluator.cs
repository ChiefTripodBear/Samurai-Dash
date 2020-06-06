using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitChainEvaluator : MonoBehaviour
{
    private static UnitChainEvaluator _instance;

    public static UnitChainEvaluator Instance => _instance;
    
    private List<Unit> _enemies = new List<Unit>();

    private void Start()
    {
        if (_instance == null)
            _instance = this;

        _enemies = FindObjectsOfType<Unit>().ToList();
        
        _enemies.ForEach(t => t.UnitKillHandler.OnDeath += () => _enemies.Remove(t));
    }
    
    
    public Queue<Unit> GetIntersectionsRelativeTo(Unit firstUnit, Vector2 firstStartCheckPoint, Vector2 firstRearCheckPoint)
    {
        var possibleIntersections = new List<Unit>();

        foreach (var enemy in _enemies)
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
                    if(Vector2.Distance(enemy.transform.position, intersect.Value) < 2f 
                       || Vector2.Distance(enemy.transform.position, firstStartCheckPoint) < 1f
                       || firstUnit != null && Vector2.Distance(firstUnit.transform.position, intersect.Value) < 1f) continue;
                    
                    enemy.UnitAngle.SetIntersectionPoint(intersect.Value);
                    possibleIntersections.Add(enemy);
                }
            }
        }

        if (possibleIntersections.Count <= 0) return null;
        
        var orderedIntersections = possibleIntersections.OrderBy(t =>
            Vector2.Distance(firstStartCheckPoint, t.UnitAngle.IntersectionPoint));
        
        var intersectOrder = new Queue<Unit>();

        for (var i = 0; i < orderedIntersections.Count(); i++)
        {
            intersectOrder.Enqueue(orderedIntersections.ElementAt(i));
        }
        return intersectOrder.Count > 0 ? intersectOrder : null;
    }

    public void RemoveEnemyFromPotentialChains(Unit currentIntersectingUnit)
    {
        if(_enemies.Contains(currentIntersectingUnit))
            _enemies.Remove(currentIntersectingUnit);
    }
}