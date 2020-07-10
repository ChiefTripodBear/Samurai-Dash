using System;
using UnityEngine;

public class PlayerMover
{
    public static event Action OnArrival;
    public static event Action RequestedNewPackage;
    public static event Action OnFirstMove;

    public event Action<MovementPlan, Action<MovementPlan>, bool, bool> MovementPlanRequested;
    private readonly Transform _mover;
    private MovementPlan _currentPlan;
    private float _distanceToCurrentDestination;
    private float _currentMoveSpeed;

    private readonly float _slowMoveSpeed;
    private float _defaultMoveSpeed = 40f;

    public bool IsMoving => _currentPlan != null;
    public MovementPlan CurrentPlan => _currentPlan;
    public Vector2? CurrentMoveDirection => _currentPlan?.MoveDirection;

    public PlayerMover(Transform mover)
    {
        _mover = mover;
        _slowMoveSpeed = _defaultMoveSpeed / 4;
        _currentMoveSpeed = _defaultMoveSpeed;
        // PlayerDashMonitor.NoDashCharges += Reset;
    }
    
    public void SetCurrentPlan(MovementPlan plan)
    {
        _currentPlan = plan;
        if (_currentPlan == null)
            return;
        _distanceToCurrentDestination = Vector2.Distance(_mover.position, _currentPlan.TargetLocation);
    }

    public void Tick()
    {
        if (!IsMoving)
        {
            _mover.GetComponent<Collider2D>().enabled = true;
            return;
        }

        if (_currentPlan.TargetUnit != null &&
            Vector2.Distance(_currentPlan.TargetUnit.Transform.position, _mover.position) < 3f)
        {
            _mover.GetComponent<Collider2D>().enabled = false;
        }

        if (_currentPlan.IsFirst)
        {
            OnFirstMove?.Invoke();
        }
        
        if (InRequestRange() && _currentPlan.HeadingForObstacle)
        {
            _mover.GetComponent<Collider2D>().enabled = false;

            MovementPlanRequested?.Invoke(_currentPlan, SetCurrentPlan, false, _currentPlan.HeadingForObstacle);
            _currentMoveSpeed = _defaultMoveSpeed;
        }
        
        HandleIntersection();

        HandlePotentialFinished();
        
        IncreaseSpeedOnFinalMovement();

        if (Arrived(_currentPlan.TargetLocation))
        {
            Reset();
            return;
        }

        _mover.position = Vector2.MoveTowards(_mover.position, _currentPlan.TargetLocation, _currentMoveSpeed * Time.deltaTime);
    }

    private void HandleIntersection()
    {
        if (_currentPlan.HeadingForIntersection && InRedirectRange() && !_currentPlan.HeadingForObstacle)
        {
            _mover.GetComponent<Collider2D>().enabled = false;

            _currentPlan.UpdateTargetLocationWhileMovingToIntersect(_currentPlan.TargetUnit.AngleDefinition.IntersectionPoint);

            DoTimeSlow();

            var successfulRedirect = RedirectEvaluator.ValidRedirect(
                _currentPlan.TargetUnit.Transform.position, _currentPlan.TargetLocation, 0.85f);

            if (PassedRedirectRange())
            {
                _mover.GetComponent<Collider2D>().enabled = false;

                RequestedNewPackage?.Invoke();
                MovementPlanRequested?.Invoke(_currentPlan, SetCurrentPlan, successfulRedirect, false);
                Time.timeScale = 1f;
                _currentMoveSpeed = _defaultMoveSpeed * 1.5f;
            }
        }
    }

    private void HandlePotentialFinished()
    {
        if (InRequestRange() && !_currentPlan.Finished && !_currentPlan.HeadingForIntersection && !_currentPlan.HeadingForObstacle)
        {
            _mover.GetComponent<Collider2D>().enabled = false;

            _currentMoveSpeed = _defaultMoveSpeed;
            RequestedNewPackage?.Invoke();
            MovementPlanRequested?.Invoke(_currentPlan, SetCurrentPlan, false, false);
        }
    }

    private void IncreaseSpeedOnFinalMovement()
    {
        if (_currentPlan.Finished && !_currentPlan.IsFirst) _currentMoveSpeed = _defaultMoveSpeed * 2f;
    }

    public void Reset()
    {
        OnArrival?.Invoke();
        _currentMoveSpeed = _defaultMoveSpeed;
        _currentPlan = null;
    }

    private void DoTimeSlow()
    {
        var currentDistanceToTargetLocation =
            Vector2.Distance(_mover.position, _currentPlan.TargetLocation);

        var totalDistanceTraveled = _distanceToCurrentDestination - currentDistanceToTargetLocation;

        var percent = totalDistanceTraveled / _distanceToCurrentDestination;

        Time.timeScale = Mathf.Lerp(1f, 0.1f, Mathf.Sin(percent * Mathf.PI * 0.5f));
        _currentMoveSpeed = _slowMoveSpeed;
    }

    
    private bool Arrived(Vector2 destination) => (Vector2)_mover.position == destination;
    private bool InRequestRange() => Vector2.Distance(_mover.position, _currentPlan.TargetLocation) < 0.3f;
    private bool InRedirectRange() => Vector2.Distance(_mover.position, _currentPlan.TargetLocation) < _distanceToCurrentDestination / 4;
    private bool PassedRedirectRange() => Vector2.Distance(_mover.position, _currentPlan.TargetLocation) < 0.1f;

}