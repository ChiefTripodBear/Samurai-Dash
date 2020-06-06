using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMover : MonoBehaviour
{
    public static event Action OnDestinationReached;
    [SerializeField] private float _dangerRedirectRadiusCheck = 1f;
    [SerializeField] private float _optimalDistancePostKill = 5f;
    [SerializeField] private float _moveAmountPerSwipe;
    [SerializeField] private float _moveTime;
    
    private float _currentMoveTime;
    private bool _isMoving;
    private Vector2 _startPosition;
    private Vector2 _endPosition;
    private Vector2 _moveDirection;
    private Vector2 _destination;

    private TargetEvaluator _targetEvaluator;

    private Unit _currentUnit;
    private LayerMask _enemyLayer;

    private void Awake()
    {
        _enemyLayer = LayerMask.GetMask("Enemy");
        _targetEvaluator = GetComponent<TargetEvaluator>();
    }

    private void Update()
    {
        if (!_isMoving && Mouse.current.leftButton.wasPressedThisFrame)
            _startPosition = Mouse.current.position.ReadValue();

        if (!_isMoving && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            _endPosition = Mouse.current.position.ReadValue(); 
            _moveDirection = (_endPosition - _startPosition).normalized;

            var enemy = _targetEvaluator.EnemyInMoveDirection(transform, _moveAmountPerSwipe, _moveDirection);

            if (enemy != null)
            {
                if (Vector2.Distance(enemy.transform.position, transform.position) > 1f)
                {
                    if (_targetEvaluator.DotProductSuccess(enemy, _moveDirection))
                    {
                        GetComponent<BoxCollider2D>().enabled = false;

                        _currentUnit = enemy;
                        var checkPoint = _currentUnit.UnitAngle.GetPointClosestTo(transform.position);
                        _currentUnit.UnitKillHandler.SetKillPoint();
                        _moveDirection = ((Vector2) _currentUnit.transform.position - checkPoint).normalized;
                        _destination = checkPoint;
                    }
                    else
                    {
                        _destination = (Vector2) transform.position + _moveDirection * _moveAmountPerSwipe;
                    }   
                }
                else
                {
                    _destination = (Vector2) transform.position + _moveDirection * _moveAmountPerSwipe;
                }
            }
            else
            {
                _destination = (Vector2) transform.position + _moveDirection * _moveAmountPerSwipe;
            }
            
            StartCoroutine(MoveTo());
        }
    }

    private Unit _nextUnit;
    private bool _evaluating;
    private List<Unit> _intersectingEnemies = new List<Unit>();

    private IEnumerator MoveTo()
    {
        _isMoving = true;
        while ((Vector2)transform.position != _destination)
        {
            if (Vector2.Distance(transform.position, _destination) > 0.3f)
            {
                if (InDangerAtTargetLocation(_destination))
                {
                    yield return StartCoroutine(FinishLerpAndOfferSafetyRedirect(_destination));
                }
            }
            // If we're not already evaluating an enemy and it's potential targets, We really only want to check this if we don't already have a "target enemy".
            // If we check more than once per frame we'll end up resetting our current trajectory.  _evaluating is used to trigger when the function can be called. 
            // This function determines what our next move is.  If we don't already have a target coming into the function, or we do but we didn't find a parallel enemy or 
            // a set of intersections, we already have our destination to move towards.
            if (_currentUnit != null && _nextUnit == null && Vector2.Distance(transform.position, _destination) < 0.1f && !_evaluating)
            {
                GetComponent<BoxCollider2D>().enabled = false;
                _evaluating = true;
                var parallelEnemy = _targetEvaluator.GetParallelEnemyFromTargetLocation(_currentUnit.UnitKillHandler.KillPoint.Value,
                    _optimalDistancePostKill, _moveDirection);

                var intersections = UnitChainEvaluator.Instance.GetIntersectionsRelativeTo(_currentUnit, _currentUnit.transform.position, _currentUnit.UnitAngle.RearPointRelative);

                _intersectingEnemies.Clear();
                RedirectDisplayManager.Instance.ResetDisplay();
                if (intersections != null)
                {
                    _intersectingEnemies = intersections.ToList();

                    for (var i = 0; i < _intersectingEnemies.Count; i++)
                    {
                        if(i == 0)
                            RedirectDisplayManager.Instance.SetActiveDisplayWithIntersectionAt(_intersectingEnemies[i].UnitAngle.IntersectionPoint);
                        RedirectDisplayManager.Instance.DisplayCorrectVector(
                            (_intersectingEnemies[i].UnitAngle.IntersectionPoint - (Vector2)_intersectingEnemies[i].transform.position).normalized,
                            _intersectingEnemies[i].UnitAngle.IntersectionPoint);
                    }
                }
        
                var intersectingEnemy = intersections?.Dequeue();
                
                _moveDirection = (_currentUnit.transform.position - transform.position).normalized;
                
                if (intersectingEnemy != null && parallelEnemy == null)
                {
                    RedirectDisplayManager.Instance.SetActiveDisplayWithIntersectionAt(intersectingEnemy.UnitAngle.IntersectionPoint);

                    _nextUnit = intersectingEnemy;
                    _destination = _nextUnit.UnitAngle.IntersectionPoint;

                    yield return StartCoroutine(HandleIntersection(_nextUnit, intersections));
                    _nextUnit = null;
                    _evaluating = false;
                }
                else if (parallelEnemy != null && intersectingEnemy == null)
                {
                    _nextUnit = parallelEnemy;
                    _nextUnit.UnitKillHandler.SetKillPoint();
                    _destination = _nextUnit.UnitAngle.GetPointClosestTo(transform.position);
                    _evaluating = false;
                    _currentUnit = _nextUnit;
                    _nextUnit = null;
                }
                else if (parallelEnemy != null && intersectingEnemy != null)
                {
                    var parallelEnemyDistance =
                        Vector2.Distance(parallelEnemy.transform.position, _currentUnit.transform.position);
                    var intersectionDistance =
                        Vector2.Distance(intersectingEnemy.UnitAngle.IntersectionPoint, transform.position);

                    _nextUnit = parallelEnemyDistance < intersectionDistance ? parallelEnemy : intersectingEnemy;
                    _destination = _nextUnit == parallelEnemy
                        ? _nextUnit.UnitAngle.GetPointClosestTo(transform.position)
                        : _nextUnit.UnitAngle.IntersectionPoint;

                    if (_nextUnit == intersectingEnemy)
                    {
                        yield return StartCoroutine(HandleIntersection(_nextUnit, intersections));
                        _nextUnit = null;
                        _evaluating = false;
                    }
                }
                else
                {
                    if (_currentUnit != null)
                    {
                        _destination = (Vector2)_currentUnit.transform.position + _moveDirection *
                            _optimalDistancePostKill;
                    }
                }
            }
            
            _currentMoveTime += Time.deltaTime;
            var percent = _currentMoveTime / _moveTime;

            transform.position = Vector2.Lerp(transform.position, _destination, percent);

            yield return null;
        }

        FinishMovement();
    }

    private void FinishMovement()
    {
        RedirectDisplayManager.Instance.ResetDisplay();
        OnDestinationReached?.Invoke();
        GetComponent<BoxCollider2D>().enabled = true;
        _currentUnit = null;
        _nextUnit = null;
        _evaluating = false;
        transform.position = _destination;
        _currentMoveTime = 0f;
        _isMoving = false;
    }

    private IEnumerator FinishLerpAndOfferSafetyRedirect(Vector2 destination)
    {
        var distanceUntilArrival = Vector2.Distance(transform.position, destination);

        var safetyRedirectTaken = false;
        while (transform.position != (Vector3)destination)
        {
            if (Vector2.Distance(transform.position, destination) < distanceUntilArrival / 2)
            {
                _moveTime = 50f;
                Time.timeScale = 0.1f;
                _isMoving = false;
            }

            if (!_isMoving && Mouse.current.leftButton.wasPressedThisFrame)
            {
                safetyRedirectTaken = true;
                _isMoving = true;
            }

            if (!_isMoving && Vector2.Distance(transform.position, destination) < 0.1f)
            {
                _moveTime = 0.35f;
                Time.timeScale = 1f;
                yield break;
            }

            if (safetyRedirectTaken && Mouse.current.leftButton.wasReleasedThisFrame)
            {
                _moveTime = 0.35f;
                Time.timeScale = 1f;

                _currentMoveTime = 0f;
                safetyRedirectTaken = false;
                yield break;
            }
            
            _currentMoveTime += Time.deltaTime;
            var percent = _currentMoveTime / _moveTime;

            transform.position = Vector2.Lerp(transform.position, _destination, percent);

            yield return null;
        }
        
        FinishMovement();
    }

    private bool InDangerAtTargetLocation(Vector2 destination)
    {
        return Physics2D.OverlapCircle(destination, _dangerRedirectRadiusCheck, _enemyLayer) && _currentUnit == null;
    }

    private IEnumerator HandleIntersection(Unit currentIntersectingUnit, Queue<Unit> intersections)
    {
        Vector2? currentIntersection = currentIntersectingUnit.UnitAngle.IntersectionPoint;
        while (currentIntersection != null)
        {
            if (_currentUnit == null || currentIntersectingUnit == null) yield break;
            
            // If we're in range of our redirect
            if (Vector2.Distance(transform.position, currentIntersection.Value) <
                Vector2.Distance(_currentUnit.transform.position, currentIntersection.Value) / 4)
            {
                // Check for any parallel enemies
                var parallelEnemy = _targetEvaluator.GetParallelEnemyFromTargetLocation(
                    currentIntersection.Value, _moveAmountPerSwipe, _moveDirection);

                // Slow down the player and the world.
                Time.timeScale = 0.2f;
                _moveTime = 20f;
                
                
                // Read input to evaluate the redirect.
                var mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                var directionFromIntersectToEnemy =
                    ((Vector2) currentIntersectingUnit.transform.position - currentIntersection.Value).normalized;
                var mouseDirection = ((Vector2) mousePosition - currentIntersection.Value).normalized;

                var mouseDot = Vector2.Dot(directionFromIntersectToEnemy, mouseDirection);
                
                var succeededRedirect = mouseDot > 0.85f;
                
                // Now that we're closer, time to evaluate the redirect.
                if (Vector2.Distance(transform.position, currentIntersection.Value) < 0.2f)
                {
                    UnitChainEvaluator.Instance.RemoveEnemyFromPotentialChains(currentIntersectingUnit);
                    // Revert the speed.
                    // TODO: Tune these, don't hardcode them.
                    Time.timeScale = 1f;
                    _moveTime = 0.35f;
                    _currentMoveTime = 0f;
                    
                    if (succeededRedirect)
                    {
                        // Set our move direction down the new vector towards our intersecting enemy.
                        // We mainly use the move direction as a means of detecting any parallel enemies.
                        _moveDirection =
                            ((Vector2) currentIntersectingUnit.transform.position -
                             currentIntersection.Value).normalized;

                        // Check for any parallel enemies along our path to the intersecting enemy.
                        var parallelEnemyFromIntersect =
                            _targetEvaluator.GetParallelEnemyFromTargetLocation(currentIntersection.Value,
                                _moveAmountPerSwipe, _moveDirection);

                        // If there's a parallel enemy on our way, set it's kill point - we'll inevitably pass this enemy.
                        if (parallelEnemyFromIntersect != null)
                        {
                            parallelEnemyFromIntersect.UnitKillHandler.SetKillPoint();
                        }

                        // Set the current enemy equal to this intersecting enemy.  Once we leave this if statement, we'll return
                        // to the MoveTo function and reevaluate from the perspective of our new enemy.  Before we finish here, though,
                        // Set this enemies kill point, because we're going to move through it, and set our destination to this enemy's "check" point.
                        _currentUnit = currentIntersectingUnit;
                        _currentUnit.UnitKillHandler.SetKillPoint();
                        _destination = currentIntersectingUnit.UnitAngle.GetPointClosestTo(currentIntersection.Value);
                        currentIntersection = null;
                    }
                    else
                    {
                        if (parallelEnemy != null && intersections.Count <= 0)
                        {
                            // If we failed our redirect and all that's in our path is another parallel enemy and no more intersections,
                            // set the current enemy to the parallel enemy so that we can reevaluate from it's "check point".

                            _currentUnit = parallelEnemy;
                            _currentUnit.UnitKillHandler.SetKillPoint();
                            _destination = _currentUnit.UnitAngle.GetPointClosestTo(transform.position);
                            currentIntersection = null;
                        }
                        else if (intersections.Count > 0)
                        {
                            // If we still have intersections on this path, set up the next intersection and notify the display to set the active indicator
                            // to this new intersection.
                            var nextIntersectingEnemy = intersections.Dequeue();
                            currentIntersectingUnit = nextIntersectingEnemy;
                            currentIntersection = nextIntersectingEnemy.UnitAngle.IntersectionPoint;
                            RedirectDisplayManager.Instance.SetActiveDisplayWithIntersectionAt(currentIntersection.Value);
                            _destination = currentIntersection.Value;
            
                            if (parallelEnemy != null)
                            {
                                // If we find a parallel enemy while we set up the new intersection, figure out if the parallel enemy is closer or if the
                                // intersection is closer.  If the intersection is closer, we don't need to worry about setting the parallel enemy's kill point yet
                                // because we might not actually move through it.  If the parallel enemy is before the intersection, we'll inevitably run through it,
                                // so, set it's kill point.
                                var parallelEnemyDistance = Vector2.Distance(parallelEnemy.transform.position,
                                    _currentUnit.transform.position);

                                var nextIntersectionDistance = Vector2.Distance(nextIntersectingEnemy.UnitAngle.IntersectionPoint,
                                    _currentUnit.transform.position);

                                // Parallel before intersection, set the kill point, we will inevitably pass this enemy
                                if (parallelEnemyDistance < nextIntersectionDistance)
                                {
                                    parallelEnemy.UnitKillHandler.SetKillPoint();
                                }
                            }
                        }
                        else if (parallelEnemy == null && intersections.Count <= 0)
                        {
                            // We've run out of enemies and intersections to move through; set the current enemy to null so that when we leave this routine
                            // we won't set the destination relative to the current enemy, but instead, to the previous intersection.  If we don't have an intersecting enemy,
                            // or a parallel enemy to work from, our current enemy was the first enemy we evaluated, which is not the position we want to establish a destination from.
                            
                            _currentUnit = null;
                            _destination = currentIntersection.Value +
                                           _moveDirection * _moveAmountPerSwipe;
                            currentIntersection = null;
                        }
                    }
                }
            }

            
            // Move!
            // TODO: Find a nice interpolation algorithm to satisfy our desired behavior.
            _currentMoveTime += Time.deltaTime;
            var percent = _currentMoveTime / _moveTime;

            transform.position = Vector2.Lerp(transform.position, _destination, percent);

            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(_destination, .5f);
        
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + _moveDirection * 5f);
    }
}
