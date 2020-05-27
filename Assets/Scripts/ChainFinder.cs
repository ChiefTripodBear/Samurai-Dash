using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChainFinder : MonoBehaviour
{
    [SerializeField] private float _minCheckRange = 5f;
    private List<Enemy> _enemies = new List<Enemy>();
    
    private Enemy _currentlyEvaluated;

    private void Awake()
    {
        _enemies = FindObjectsOfType<Enemy>().ToList();
        
        _enemies.ForEach(t => t.OnDeath += () => _enemies.Remove(t));
    }
    
    [SerializeField] private float _intersectionSafetyDistance = 1f;
    private Vector2? _previousIntersection;
    private Vector2? _currentRearProjection;

    public Queue<Enemy> BuildChain(Enemy startingEnemy)
    {
        var chain = new Queue<Enemy>();

        // Add the first enemy the player attacked to the queue, we'll use it to do our initial evaluations.
        chain.Enqueue(startingEnemy);

        var currentlyEvaluatedEnemy = startingEnemy;

        while (currentlyEvaluatedEnemy != null)
        {
            // We'll use this to determine if we've actually added anyone to our chain.
            var chainCount = chain.Count;

            AssignPredictiveRearProjection(currentlyEvaluatedEnemy);

            // Evaluate by distance to the current enemy.  May need to find a better algorithm at some point.  Maybe calculate
            // by number of links in the chain?
            var orderedByRange = OrderEnemiesBy(chain, currentlyEvaluatedEnemy);

            // Loop through all enemies in range.
            foreach (var enemy in orderedByRange)
            {
                // Check to see if an intersection is possible from the enemy under evaluation, it's rear point (we only want check the side that
                // the player will end up at), and the closest eligible enemy's forward and rear projection points.
                var doesIntersect = DoesIntersect(currentlyEvaluatedEnemy, enemy);

                if (doesIntersect)
                {
                    // If it's possible to intersect, we find the exact point (within floating point error).
                    var point = GetIntersectionPoint(currentlyEvaluatedEnemy, enemy);

                    if (point != null)
                    // If we've found our intersect, we need to make sure that it's actually sitting on the right side of the enemy.
                    // It's possible that we found an intersection on the same side as our start side.
                    // The intersection needs to be on the rear side of the enemy at the time the player gets to the previous intersection.
                        if (IntersectionPointSitsOnRearSegmentBeforeTimeOfIntersect(currentlyEvaluatedEnemy,
                            _currentRearProjection.Value, point.Value))
                        {
                            // If the intersection point is on the right side, but it's too close to either enemies, it won't work.
                            if (!IntersectionPointTooCloseToEnemy(point.Value, enemy, currentlyEvaluatedEnemy))
                            {
                                // Store the value of this intersection so that we can use it for the next enemy to figure out where their rear side will be
                                // when the enemy makes it to this intersection.
                                _previousIntersection = point.Value;
                                
                                // We've found ourselves a new enemy to evaluate!  The chain may grow longer!
                                currentlyEvaluatedEnemy.EnemyChainHandler.SetIntersectionPoint(point);
                                currentlyEvaluatedEnemy = enemy;
                                
                                // Stuff the current enemy into our chain.  It will store all of the relevant points for the player.
                                chain.Enqueue(currentlyEvaluatedEnemy);
                                break;
                            }
                        }
                }
            }

            // If, after all of our looping, we didn't find one enemy to add to the chain, exit because there's no enemies left to evaluate.
            if (chain.Count <= chainCount) break;
        }

        _previousIntersection = null;
        return chain.Count > 1 ? chain : null;
    }

    private Vector2? GetIntersectionPoint(Enemy currentlyEvaluatedEnemy, Enemy enemy)
    {
        return IntersectionMaths.IntersectionPoint(currentlyEvaluatedEnemy.transform.position,
            _currentRearProjection.Value,
            enemy.EnemyAngleDefinition.ForwardProjectionPoint,
            enemy.EnemyAngleDefinition.RearProjectionPoint);
    }

    private bool DoesIntersect(Enemy currentlyEvaluatedEnemy, Enemy enemy)
    {
        var doesIntersect =
            IntersectionMaths.IsIntersecting(currentlyEvaluatedEnemy.transform.position,
                _currentRearProjection.Value,
                enemy.EnemyAngleDefinition.ForwardProjectionPoint,
                enemy.EnemyAngleDefinition.RearProjectionPoint);
        return doesIntersect;
    }

    private IOrderedEnumerable<Enemy> OrderEnemiesBy(Queue<Enemy> chain, Enemy currentlyEvaluatedEnemy)
    {
        var orderedByRange = _enemies
            .Where(t => !chain.Contains(t) &&
                        Vector2.Distance(t.transform.position, currentlyEvaluatedEnemy.transform.position) <
                        _minCheckRange)
            .OrderBy(t => Vector2.Distance(currentlyEvaluatedEnemy.transform.position, t.transform.position));
        return orderedByRange;
    }

    private void AssignPredictiveRearProjection(Enemy currentlyEvaluatedEnemy)
    {
        // Used to determine which side of the enemy the intersection needs to be on.
        if (_previousIntersection != null)
            _currentRearProjection =
                currentlyEvaluatedEnemy.EnemyAngleDefinition.GetRearProjectionAtTimeOfIntersection(_previousIntersection
                    .Value);
        else
        // The first enemy evaluated.  No intersections have happened yet. 
            _currentRearProjection =
                currentlyEvaluatedEnemy.EnemyAngleDefinition.RearProjectionPoint;
    }

    private bool IntersectionPointSitsOnRearSegmentBeforeTimeOfIntersect(Enemy currentlyEvaluatedEnemy,
        Vector2 currentRearProjection, Vector2 point)
    {
        return IntersectionMaths.PointLiesOnSegment(point, currentlyEvaluatedEnemy.transform.position, currentRearProjection);
    }

    private bool IntersectionPointTooCloseToEnemy(Vector2 intersection, Enemy newEnemy, Enemy oldEnemy)
    {
        return Vector2.Distance(intersection, newEnemy.transform.position) < _intersectionSafetyDistance 
               || Vector2.Distance(intersection, oldEnemy.transform.position) < _intersectionSafetyDistance;
    }
}