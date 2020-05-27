using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMover : MonoBehaviour
{
    public event Action<bool> MoveThroughChainStatusChanged;
    [SerializeField] private TrailRenderer _trailRenderer;
    [SerializeField] private float _optimalDistancePostKill;
    [SerializeField] private float _moveAmountPerSwipe = 5f;
    [SerializeField] private float _moveTime;
    [SerializeField] private float _minimumAttackDistance = 1f;
    [SerializeField] private float _dangerCheckRadius;

    private Vector2 _startingPoint;
    private Vector2 _endPoint;
    private Vector2 _moveDirection;
    private Vector2? _targetLocation;
    private Vector2? _finalChainLocation;

    private float _currentMoveTime;
    private bool _isMoving;

    private PlayerEnemyFinder _playerEnemyFinder;
    private ChainFinder _chainFinder;

    private Enemy _currentChainTarget;
    private Coroutine _moveRoutine;
    private LayerMask _enemyLayer;

    private void Awake()
    {
        _enemyLayer = LayerMask.GetMask("Enemy");
        _trailRenderer.enabled = false;
        _playerEnemyFinder = GetComponent<PlayerEnemyFinder>();
        _chainFinder = FindObjectOfType<ChainFinder>();
    }

    private void Update()
    {
        SetUpNextMove();
    }
    
    /// <summary>
    /// Sets both the initial values required for movement
    /// and any other movement specifications - i.e. optimal distance post kill, provides checks based on the dot product and distance
    /// checks for any enemy that's been found to ensure we have a valid target based on our movement.  Calls a standard movement Coroutine
    /// and a chain-based Coroutine depending on the outcome of the movement.
    /// </summary>
    private void SetUpNextMove()
    {
        if (!_isMoving && Mouse.current.leftButton.wasPressedThisFrame) _startingPoint = Mouse.current.position.ReadValue();
        
        if (!_isMoving && Mouse.current.leftButton.wasReleasedThisFrame)
        { 
            SetIntialMovementValues();

            var enemyInMoveRange = _playerEnemyFinder.CheckForEnemyInFrontAtPosition(transform, _moveAmountPerSwipe, _moveDirection);
            
            if (NoTargetAvailable(enemyInMoveRange) || DotFailed(enemyInMoveRange))
            {
                StartCoroutine(NormalMovement(_targetLocation.Value, null));
                return;
            }

            // Gain IFrames for the duration of the move because we're killing this SOB.
            GetComponent<BoxCollider2D>().enabled = false;

            var distanceFromOptimalLocation = GetOptimalDistanceFromEnemyAfterKill(enemyInMoveRange, _targetLocation.Value);
            _targetLocation += _moveDirection * distanceFromOptimalLocation;
            
            var chain = _chainFinder.BuildChain(enemyInMoveRange);
            
            StartCoroutine(chain != null  ? MoveThroughChain(chain) : NormalMovement(_targetLocation.Value, enemyInMoveRange));
        }
    }
    /// <summary>
    /// Get our end point.  With the end point, create our move direction and subsequent target location.
    /// </summary>
    private void SetIntialMovementValues()
    {
        _endPoint = Mouse.current.position.ReadValue();
        _moveDirection = (_endPoint - _startingPoint).normalized;
        _targetLocation = (Vector2) transform.position + _moveDirection * _moveAmountPerSwipe;
    }
    
    /// <summary>
    /// Either the enemy wasn't found (normal movement with no enemy in front OR the enemy was found but it was too close.
    /// </summary>
    /// <param name="enemyInMoveRange"></param>
    /// <returns></returns>
    private bool NoTargetAvailable(Enemy enemyInMoveRange)
    {
        return enemyInMoveRange == null;
    }

    /// <summary>
    /// If the dot between our enemy's kill vector and our move direction is less than the desired threshold, our attack will fail.
    /// </summary>
    /// <param name="enemyInMoveRange"></param>
    /// <returns></returns>
    private bool DotFailed(Enemy enemyInMoveRange)
    {
        return enemyInMoveRange != null && Vector2.Dot(_moveDirection.normalized, enemyInMoveRange.EnemyAngleDefinition.KillVector) < 1 - .1f;
    }
    
    /// <summary>
    /// Find the distance we need to travel to make it to our target location.
    /// This might be a good place take into consideration how close we will be to an enemy.
    /// </summary>
    /// <param name="enemy"></param>
    /// <param name="targetLocation"></param>
    /// <returns></returns>
    ///TODO: Determine how close we'll be to other enemies, if we're too close, find a more suitable position.
    private float GetOptimalDistanceFromEnemyAfterKill(Enemy enemy, Vector2 targetLocation)
    {
        return _optimalDistancePostKill - Vector2.Distance(enemy.transform.position, targetLocation);
    }
    
    /// <summary>
    /// Standard time/percent-based lerp.
    /// TODO: Play with different interpolation. 
    /// </summary>
    /// <param name="targetLocation"></param>
    private void Lerp(Vector2 targetLocation)
    {
        _currentMoveTime += Time.deltaTime;

        if (_currentMoveTime >= _moveTime)
            _currentMoveTime = _moveTime;

        var percentComplete = _currentMoveTime / _moveTime;

        transform.position = Vector2.Lerp(transform.position, targetLocation, percentComplete);
    }

    
    /// <summary>
    /// Move towards the targetLocation via lerp  If we had an enemy going into the movement, kill the enemy once we've finished the movement.
    /// </summary>
    /// <param name="targetLocation"></param>
    /// <param name="enemyInMoveRange"></param>
    /// <returns></returns>
    private IEnumerator NormalMovement(Vector2 targetLocation, Enemy enemyInMoveRange)
    {
        _currentMoveTime = 0f;
        _isMoving = true;

        while (Vector2.Distance(transform.position, targetLocation) > 0.3f)
        {
            if (InDangerAtTargetLocation(ref targetLocation))
            {
                yield return StartCoroutine(FinishLerpAndOfferSafetyRedirect(targetLocation, enemyInMoveRange));
                yield break;
            }
            
            Lerp(targetLocation);

            yield return null;
        }

        if (enemyInMoveRange != null)
        {
            enemyInMoveRange.Kill();
        }
        ResetMove();
    }

    private IEnumerator FinishLerpAndOfferSafetyRedirect(Vector2 targetLocation, Enemy previousEnemy)
    {
        var distanceUntilArrival = Vector2.Distance(transform.position, targetLocation);

        var safetyRedirectTaken = false;
        while (true)
        {
            if (Vector2.Distance(transform.position, targetLocation) < distanceUntilArrival / 2)
            {
                if(previousEnemy != null)
                    previousEnemy.Kill();
                _moveTime = 50;
                Time.timeScale = 0.1f;
                _isMoving = false;
            }

            if (!_isMoving && Mouse.current.leftButton.wasPressedThisFrame)
            {
                safetyRedirectTaken = true;
                _isMoving = true;
            }

            if (safetyRedirectTaken && Mouse.current.leftButton.wasReleasedThisFrame)
            {
                _moveTime = .35f;
                Time.timeScale = 1f;
                
                _currentMoveTime = 0f;
                _trailRenderer.enabled = false;
                safetyRedirectTaken = false;
                yield break;
            }

            Lerp(targetLocation);

            yield return null;
        }
    }

    /// <summary>
    /// Resets all relevant movement values.
    /// </summary>
    private void ResetMove()
    {
        Time.timeScale = 1f;
        _trailRenderer.enabled = false;
        _isMoving = false;
        if (_targetLocation != null) transform.position = _targetLocation.Value;
        _targetLocation = null;
        _finalChainLocation = null;
        _currentMoveTime = 0f;
        GetComponent<BoxCollider2D>().enabled = true;
    }

    private bool _willBeInDangerAtTargetLocation = false;

    /// <summary>
    /// Uses the chain developed by the Chain Finder to create a path to each of the links' intersection points.
    /// Finds the best exit point.
    /// </summary>
    /// <param name="chain"></param>
    /// <returns></returns>
    private IEnumerator MoveThroughChain(Queue<Enemy> chain)
    {
        MoveThroughChainStatusChanged?.Invoke(true);
        
        _isMoving = true;
        _currentChainTarget = chain.Dequeue();
        
        var targetLocation = _currentChainTarget.EnemyChainHandler.IntersectionPoint.Value;
        
        while (true)
        {
            Debug.Log(_currentChainTarget);
            var newTarget = ValidTargetWhileMoving(_currentChainTarget);
            _moveDirection = (targetLocation - (Vector2) transform.position).normalized;

            _targetLocation = targetLocation;
            _trailRenderer.enabled = true;
            Time.timeScale = .3f;

            // If we've arrived at the target location - an intersection - check the chain to see if we have more intersect locations.
            if (Vector2.Distance(transform.position, targetLocation) < 0.1f)
            {
                if (newTarget != null)
                {
                    Debug.Log("Killing new enemy");
                    newTarget.Kill();
                }
                SetChainExitPoint(chain, targetLocation);
                if (ChainComplete(chain, ref targetLocation)) break;
            }

            Lerp(targetLocation);

            yield return null;
        }
        

        MoveThroughChainStatusChanged?.Invoke(false);
        
        // Do final movement to exit the chain.
        _targetLocation = _finalChainLocation;

        yield return StartCoroutine(NormalMovement(_targetLocation.Value, _currentChainTarget));
    }

    private Enemy ValidTargetWhileMoving(Enemy currentTarget)
    {
        var newEnemy = _playerEnemyFinder.CheckForEnemyInFrontAtPosition(currentTarget.transform, _moveAmountPerSwipe, _moveDirection);

        if (newEnemy != null && newEnemy != currentTarget)
        {
            return DotFailed(newEnemy) ? null : newEnemy;
        }

        return null;
    }
    
    private bool InDangerAtTargetLocation(ref Vector2 destination)
    {
        // TODO: Check if we can kill the enemy, if so we'll need to change the destination to an optimal position away from the new enemy.
        return Physics2D.OverlapCircle(destination, 1f, _enemyLayer);
    }


    /// <summary>
    /// Checks if we've completed the chain and handles setting the next destination in the chain.
    /// If the chain isn't complete, we kill the previous enemy and set the timescale back to one.
    /// Called once we've arrived at the previous destination so we destroy that enemy.
    /// </summary>
    /// <param name="chain"></param>
    /// <param name="targetLocation"></param>
    /// <returns></returns>
    private bool ChainComplete(Queue<Enemy> chain, ref Vector2 targetLocation)
    {
        // Stuff the current enemy back in the pool.
        _currentChainTarget.Kill();

        // If we've got more points to move through.
        if (chain.Count > 0)
        {
            // Get the next target.
            _currentChainTarget = chain.Dequeue();

            // If we're on the last enemy in the queue, it won't have an intersection point.  In that case we'll break out and finish
            // the movement by exiting through normal means.  
            if (_currentChainTarget.EnemyChainHandler.IntersectionPoint != null)
            {
                // We have another location from the chain, the chain continues.
                targetLocation = _currentChainTarget.EnemyChainHandler.IntersectionPoint.Value;
            }
            else
            {
                Time.timeScale = 1f;
                // No more intersection points, exit via Normal Movement method.
                return true;
            }
        }
        else
        {
            Time.timeScale = 1f;
            return true;
        }

        _currentMoveTime = 0f;
        return false;
    }

    /// <summary>
    /// Sets the destination once we've completed the chain.
    /// </summary>
    /// <param name="chain"></param>
    /// <param name="targetLocation"></param>
    private void SetChainExitPoint(Queue<Enemy> chain, Vector2 targetLocation)
    {
        var lastEnemyInChain = chain.LastOrDefault();

        if (lastEnemyInChain != null)
        {
            _moveDirection = ((Vector2) lastEnemyInChain.transform.position - targetLocation).normalized;
            _finalChainLocation = (Vector2) lastEnemyInChain.transform.position +
                                  _moveDirection * _optimalDistancePostKill;
        }
    }

    private void OnDrawGizmos()
    {
        if (_targetLocation != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_targetLocation.Value, 1f);
        }

        if (_finalChainLocation != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_finalChainLocation.Value, 1f);
        }
    }
}