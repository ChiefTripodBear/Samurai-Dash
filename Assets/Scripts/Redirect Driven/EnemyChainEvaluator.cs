using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyChainEvaluator : MonoBehaviour
{
    private static EnemyChainEvaluator _instance;

    public static EnemyChainEvaluator Instance => _instance;
    
    private List<EnemyAngle> _enemies = new List<EnemyAngle>();

    private void Awake()
    {
        if (_instance == null)
            _instance = this;

        _enemies = FindObjectsOfType<EnemyAngle>().ToList();
        
        _enemies.ForEach(t => t.OnDeath += () => _enemies.Remove(t));
    }
    
    
    public Queue<EnemyAngle> GetIntersectionsRelativeTo(EnemyAngle firstEnemy, Vector2 firstStartCheckPoint, Vector2 firstRearCheckPoint)
    {
        var possibleIntersections = new List<EnemyAngle>();

        foreach (var enemy in _enemies)
        {    
            if(firstEnemy != null && enemy == firstEnemy) continue;

            var rearPoint = enemy.RearPointRelative;
            var forwardPoint = enemy.ForwardPointRelative;
            if(IntersectionMaths.IsIntersecting(firstStartCheckPoint, firstRearCheckPoint, rearPoint, forwardPoint))
            {
                var intersect = IntersectionMaths.FindIntersection(firstStartCheckPoint, firstRearCheckPoint,
                    rearPoint, forwardPoint);

                if (intersect != null)
                {
                    if(Vector2.Distance(enemy.transform.position, intersect.Value) < 2f 
                       || Vector2.Distance(enemy.transform.position, firstStartCheckPoint) < 1f
                       || firstEnemy != null && Vector2.Distance(firstEnemy.transform.position, intersect.Value) < 1f) continue;
                    
                    enemy.SetIntersectionPoint(intersect.Value);
                    possibleIntersections.Add(enemy);
                }
            }
        }

        if (possibleIntersections.Count <= 0) return null;
        
        var orderedIntersections = possibleIntersections.OrderBy(t =>
            Vector2.Distance(firstStartCheckPoint, t.IntersectionPoint));
        
        var intersectOrder = new Queue<EnemyAngle>();

        for (var i = 0; i < orderedIntersections.Count(); i++)
        {
            intersectOrder.Enqueue(orderedIntersections.ElementAt(i));
        }
        return intersectOrder.Count > 0 ? intersectOrder : null;
    }

    public void RemoveEnemyFromPotentialChains(EnemyAngle currentIntersectingEnemy)
    {
        if(_enemies.Contains(currentIntersectingEnemy))
            _enemies.Remove(currentIntersectingEnemy);
    }
}