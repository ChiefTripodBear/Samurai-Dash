using System;
using UnityEngine;

public class PlayerSafetyRedirect
{
    public static event Action InDanger;
    public bool SafetyRedirectAllowed { get; set; }
    private Mover _mover;
    private readonly Player _player;
    private float _safetyCheckRange = 4f;

    private float _safetyRedirectMoveSpeed = 10f;

    public PlayerSafetyRedirect(Mover mover, Player player)
    {
        _mover = mover;
        _player = player;
    }

    // public void Tick()
    // {
    //     if (_mover.MovementPackage == null || !_mover.IsMoving)
    //     {
    //         _mover.Reset();
    //         return;
    //     }
    //     
    //     if (InSafetyRedirectRange() && _mover.MovementPackage.Destination?.Unit == null && _mover.MovementPackage.Destination?.DestinationType != DestinationType.Intersection || InSafetyRedirectRange() && _mover.MovementPackage.Finished)
    //     {
    //         if (InDangerAtTargetLocation())
    //         {
    //             InDanger?.Invoke();
    //             SafetyRedirectAllowed = true;
    //             Time.timeScale = .1f;
    //             _mover.SetMoveSpeed(_safetyRedirectMoveSpeed);
    //             _mover.IsMoving = false;
    //         }
    //     }
    // }
    //
    // private bool InDangerAtTargetLocation()
    // {
    //     var targetPosition = _mover.MovementPackage.RequiredCollisionHandling
    //         ? _mover.MovementPackage.LocationBeforeCollisionAdjustment
    //         : _mover.MovementPackage.Destination.TargetLocation;
    //     var distanceToTargetLocation =
    //         Vector2.Distance(targetPosition, _player.transform.position);
    //     return _mover.MovementPackage?.Destination != null && TargetDetector.FoundInvalidEnemy(distanceToTargetLocation + distanceToTargetLocation * 0.125f, _mover.MovementPackage.Destination.MoveDirection,
    //         _player.transform.position);
    // }

    private bool InSafetyRedirectRange()
    {
        var targetPosition = _mover.MovementPackage.RequiredCollisionHandling
            ? _mover.MovementPackage.LocationBeforeCollisionAdjustment
            : _mover.MovementPackage.Destination.TargetLocation;
        return Vector2.Distance(_player.transform.position, targetPosition) < _safetyCheckRange;
    }
}