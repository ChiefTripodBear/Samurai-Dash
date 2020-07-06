using System;
using System.Collections;
using UnityEngine;

public class Mover
{
    public static event Action OnArrival;
    public static event Action<DestinationType> OnNewMovementStart;
    public static event Action OnFirstMove;
    public static event Action OnKill;
    public event Action<MovementPackage, Action<MovementPackage>, bool> NewMovementPackageRequested;
    public bool IsMoving { get; set; }
    public bool CanMove { get; set; } = true;
    public bool HasDestination => _movementPackage?.Destination != null;

    private float _defaultMoveSpeed;
    private float _slowSpeed;
    private float _currentMoveSpeed;
    
    private MovementPackage _movementPackage;
    public MovementPackage MovementPackage => _movementPackage;

    private float _distanceToCurrentDestination;
    private bool _requested;
    private int _boundaryPathIndex = 0;
    private float _currentDistanceFromTargetLocation;
    
    private Player _player;
    private Transform _transform;
    private ParallelMovingCheck _parallelMovingCheck;
    private bool _isInDanger;

    public Mover(Player player, float defaultMoveSpeed)
    {
        _player = player;
        _transform = _player.transform;
        _defaultMoveSpeed = defaultMoveSpeed;
        _slowSpeed = defaultMoveSpeed / 4;
        _currentMoveSpeed = defaultMoveSpeed;
        PlayerSafetyRedirect.InDanger += HandleDanger;
    }

    private void HandleDanger()
    {
        _isInDanger = true;
    }

    public void SetMovementPackage(MovementPackage movementPackage)
    {
        _requested = false;
        _movementPackage = movementPackage;

        if (movementPackage?.Destination != null)
        {
            _distanceToCurrentDestination = Vector2.Distance(_transform.position, _movementPackage.Destination.TargetLocation);
            OnNewMovementStart?.Invoke(_movementPackage.Destination.DestinationType);

            if (MovementPackage.MovementCount % 2 != 0)
            {
                OnFirstMove?.Invoke();   
            }
            else
            {
                OnKill?.Invoke();
            }
        }
    }

    public void Move()
    {
        if (!CanMove) return;

        _parallelMovingCheck.Tick();

        if (_movementPackage.Destination.DestinationType == DestinationType.BoundaryPath)
        {
            _player.StartCoroutine(MoveThroughBoundaryPath());
        }
        
        if (_movementPackage.Destination.DestinationType == DestinationType.Intersection && !_movementPackage.Finished)
            if (InRedirectRange())
            {
                _movementPackage.Destination.TargetLocation = _movementPackage.IntersectionAnalysis.IntersectingUnit
                    .AngleDefinition.IntersectionPoint;

                HandleIntersectionTimeSlow();
                var successfulRedirect = RedirectEvaluator.ValidRedirect(
                    _movementPackage.IntersectionAnalysis.IntersectingUnit.Transform.position,
                    _movementPackage.Destination.TargetLocation, 0.85f);
                
                if (PassedRedirectRange())
                {
                    NewMovementPackageRequested?.Invoke(_movementPackage, SetMovementPackage, successfulRedirect);
                    Time.timeScale = 1f;
                    _currentMoveSpeed = _defaultMoveSpeed * 1.5f;
                }
            }

        if (InRequestRange() && _movementPackage.Destination.DestinationType == DestinationType.ClosestPointToEnemy && !_movementPackage.Finished)
            if (!_requested)
            {
                _requested = true;
                NewMovementPackageRequested?.Invoke(_movementPackage, SetMovementPackage, false);
            }

        if (_movementPackage.Finished && !_isInDanger)
        {
            _currentMoveSpeed = _defaultMoveSpeed * 2f;
        }

        if (Arrived(_movementPackage.Destination.TargetLocation))
        {
            OnArrival?.Invoke();
            Reset();
            return;
        }
        
        IsMoving = true;
        _transform.position = Vector2.MoveTowards(_transform.position, _movementPackage.Destination.TargetLocation, _currentMoveSpeed * Time.deltaTime);
    }

    private void HandleIntersectionTimeSlow()
    {
        var currentDistanceToTargetLocation =
            Vector2.Distance(_transform.position, _movementPackage.Destination.TargetLocation);

        var totalDistanceTraveled = _distanceToCurrentDestination - currentDistanceToTargetLocation;

        var percent = totalDistanceTraveled / _distanceToCurrentDestination;

        Time.timeScale = Mathf.Lerp(1f, 0.1f, Mathf.Sin(percent * Mathf.PI * 0.5f));
        _currentMoveSpeed = _slowSpeed;
    }

    private IEnumerator MoveThroughBoundaryPath()
    {
        CanMove = false;
        // Time.timeScale = .5f;
        while (true)
        {
            if (_movementPackage == null)
            {
                _requested = true;
                CanMove = true;
                _boundaryPathIndex = 0;
                Time.timeScale = 1;
                yield break;
            }
            
            if (_boundaryPathIndex >= _movementPackage.BoundaryPathFinder.BoundaryPath.Count - 1)
            {
                _transform.position = Vector2.MoveTowards(_transform.position, _movementPackage.Destination.TargetLocation, _currentMoveSpeed * Time.deltaTime);

                if (InRequestRange())
                {
                    _movementPackage.Destination.DestinationType =
                        _movementPackage.BoundaryPathFinder.PreviousDestinationType;
                    _requested = true;
                    NewMovementPackageRequested?.Invoke(_movementPackage, SetMovementPackage, false);
                    CanMove = true;
                    _boundaryPathIndex = 0;
                    Time.timeScale = 1;
                    yield break;
                }
            }
            else
            {
                if (Vector2.Distance(_transform.position,
                    _movementPackage.BoundaryPathFinder.BoundaryPath[_boundaryPathIndex]) < 0.1f)
                {
                    _boundaryPathIndex++;
                }

                _transform.position = Vector2.MoveTowards(_transform.position, _movementPackage.BoundaryPathFinder.BoundaryPath[_boundaryPathIndex], _currentMoveSpeed * Time.deltaTime);
            }

            yield return null;
        }
    }

    private bool Arrived(Vector2 destination) => (Vector2)_transform.position == destination;
    private bool InRequestRange() => Vector2.Distance(_transform.position, _movementPackage.Destination.TargetLocation) < 0.3f;
    private bool InRedirectRange() => Vector2.Distance(_transform.position, _movementPackage.Destination.TargetLocation) < _distanceToCurrentDestination / 4;
    private bool PassedRedirectRange() => Vector2.Distance(_transform.position, _movementPackage.Destination.TargetLocation) < 0.1f;

    public void DrawGizmos()
    {
        if(_movementPackage != null)
            Gizmos.DrawWireSphere(_movementPackage.Destination.TargetLocation, 1f);

        if (_transform != null && _movementPackage != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(_transform.position, (Vector2)_transform.position + _movementPackage.Destination.MoveDirection * 5f);
        }
    }

    public void Reset()
    {
        _isInDanger = false;
        MovementPackage.MovementCount = 0;
        RedirectDisplayManager.Instance.ResetDisplay();
        _movementPackage = null;
        IsMoving = false;
        Time.timeScale = 1f;
        _currentMoveSpeed = _defaultMoveSpeed;
    }
    

    public void SetMoveSpeed(float safetyRedirectMoveSpeed)
    {
        _currentMoveSpeed = safetyRedirectMoveSpeed;
    }
}