using System;
using System.Collections;
using UnityEngine;

public class Mover
{
    public event Action<MovementPackage, Action<MovementPackage>, bool> NewMovementPackageRequested;
    public bool IsMoving { get; private set; }
    public bool CanMove { get; set; } = true;
    public bool HasDestination => _movementPackage != null;

    private float _defaultMoveSpeed;
    private float _slowSpeed;
    private float _currentMoveSpeed;

    private Transform _transform;
    
    private Destination _currentDestination;
    private MovementPackage _movementPackage;
    private float _distanceToCurrentDestination;
    private bool _requested;
    private Player _player;

    public Mover(Player player, float defaultMoveSpeed)
    {
        _player = player;
        _transform = _player.transform;
        _defaultMoveSpeed = defaultMoveSpeed;
        _slowSpeed = defaultMoveSpeed / 4;
        _currentMoveSpeed = defaultMoveSpeed;
    }

    public void SetMovementPackage(MovementPackage movementPackage)
    {
        _requested = false;
        _movementPackage = movementPackage;
        
        _distanceToCurrentDestination = Vector2.Distance(_transform.position, _movementPackage.Destination.TargetLocation);
    }

    public void Move()
    {
        if (!CanMove) return;

        if (_movementPackage.Destination.DestinationType == DestinationType.Intersection && !_movementPackage.Finished)
            if (InRedirectRange())
            {
                Time.timeScale = 0.2f;
                _currentMoveSpeed = _slowSpeed;

                var successfulRedirect = RedirectEvaluator.ValidRedirect(
                    _movementPackage.IntersectionAnalysis.IntersectingUnit.Transform.position,
                    _movementPackage.Destination.TargetLocation, 0.85f);
                
                if (PassedRedirectRange())
                {
                    NewMovementPackageRequested?.Invoke(_movementPackage, SetMovementPackage, successfulRedirect);
                    Time.timeScale = 1f;
                    _currentMoveSpeed = _defaultMoveSpeed * 2f;
                }
            }

        if (InRequestRange() && _movementPackage.Destination.DestinationType == DestinationType.ClosestPointToEnemy && !_movementPackage.Finished)
            if (!_requested)
            {
                _requested = true;
                NewMovementPackageRequested?.Invoke(_movementPackage, SetMovementPackage, false);
            }

        if (_movementPackage.Finished)
        {
            _currentMoveSpeed = _defaultMoveSpeed * 2f;
        }

        if (Arrived(_movementPackage.Destination.TargetLocation))
        {
            MovementPackage.MovementCount = 0;
            RedirectDisplayManager.Instance.ResetDisplay();
            _movementPackage = null;
            IsMoving = false;
            _currentMoveSpeed = _defaultMoveSpeed;
            return;
        }
        
        IsMoving = true;
        _transform.position = Vector2.MoveTowards(_transform.position, _movementPackage.Destination.TargetLocation, _currentMoveSpeed * Time.deltaTime);
    }

    private IEnumerator IFrames()
    {
        yield return new WaitForSeconds(.1f);
    }

    private bool InRequestRange()
    {
        return Vector2.Distance(_transform.position, _movementPackage.Destination.TargetLocation) < 0.3f;
    }

    private bool InRedirectRange()
    {
        return Vector2.Distance(_transform.position, _movementPackage.Destination.TargetLocation) < _distanceToCurrentDestination / 4;
    }

    private bool PassedRedirectRange()
    {
        return Vector2.Distance(_transform.position, _movementPackage.Destination.TargetLocation) < 0.1f;
    }

    private bool Arrived(Vector2 destination)
    {
        return (Vector2)_transform.position == destination;
    }

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
}