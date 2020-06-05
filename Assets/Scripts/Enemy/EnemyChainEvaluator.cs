using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyChainEvaluator : MonoBehaviour
{
    private static EnemyChainEvaluator _instance;

    public static EnemyChainEvaluator Instance => _instance;
    
    private List<Enemy> _enemies = new List<Enemy>();

    private void Start()
    {
        if (_instance == null)
            _instance = this;

        _enemies = FindObjectsOfType<Enemy>().ToList();
        
        _enemies.ForEach(t => t.EnemyKillHandler.OnDeath += () => _enemies.Remove(t));
    }
    
    
    public Queue<Enemy> GetIntersectionsRelativeTo(Enemy firstEnemy, Vector2 firstStartCheckPoint, Vector2 firstRearCheckPoint)
    {
        var possibleIntersections = new List<Enemy>();

        foreach (var enemy in _enemies)
        {    
            if(firstEnemy != null && enemy == firstEnemy) continue;

            var rearPoint = enemy.EnemyAngle.RearPointRelative;
            var forwardPoint = enemy.EnemyAngle.ForwardPointRelative;
            if(IntersectionMaths.IsIntersecting(firstStartCheckPoint, firstRearCheckPoint, rearPoint, forwardPoint))
            {
                var intersect = IntersectionMaths.FindIntersection(firstStartCheckPoint, firstRearCheckPoint,
                    rearPoint, forwardPoint);

                if (intersect != null)
                {
                    if(Vector2.Distance(enemy.transform.position, intersect.Value) < 2f 
                       || Vector2.Distance(enemy.transform.position, firstStartCheckPoint) < 1f
                       || firstEnemy != null && Vector2.Distance(firstEnemy.transform.position, intersect.Value) < 1f) continue;
                    
                    enemy.EnemyAngle.SetIntersectionPoint(intersect.Value);
                    possibleIntersections.Add(enemy);
                }
            }
        }

        if (possibleIntersections.Count <= 0) return null;
        
        var orderedIntersections = possibleIntersections.OrderBy(t =>
            Vector2.Distance(firstStartCheckPoint, t.EnemyAngle.IntersectionPoint));
        
        var intersectOrder = new Queue<Enemy>();

        for (var i = 0; i < orderedIntersections.Count(); i++)
        {
            intersectOrder.Enqueue(orderedIntersections.ElementAt(i));
        }
        return intersectOrder.Count > 0 ? intersectOrder : null;
    }

    public void RemoveEnemyFromPotentialChains(Enemy currentIntersectingEnemy)
    {
        if(_enemies.Contains(currentIntersectingEnemy))
            _enemies.Remove(currentIntersectingEnemy);
    }
}