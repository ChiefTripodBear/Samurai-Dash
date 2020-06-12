using UnityEngine;

public class PlayerSafetyRedirect
{
    public bool SafetyRedirectAllowed { get; set; }
    private Mover _mover;
    private readonly Player _player;
    private float _safetyCheckRange = 3f;

    private float _safetyRedirectMoveSpeed = 10f;

    public PlayerSafetyRedirect(Mover mover, Player player)
    {
        _mover = mover;
        _player = player;
    }

    public void Tick()
    {
        if (_mover.MovementPackage == null || !_mover.IsMoving)
        {
            _mover.Reset();
            return;
        }
        
        if (InSafetyRedirectRange() && _mover.MovementPackage.Finished || InSafetyRedirectRange() && MovementPackage.MovementCount == 1)
        {
            if (InDangerAtTargetLocation())
            {
                SafetyRedirectAllowed = true;
                Time.timeScale = .2f;
                _mover.SetMoveSpeed(_safetyRedirectMoveSpeed);
                _mover.IsMoving = false;
            }
        }
    }

    private bool InDangerAtTargetLocation()
    {
        var distanceToTargetLocation =
            Vector2.Distance(_mover.MovementPackage.Destination.TargetLocation, _player.transform.position);
        return _mover.MovementPackage?.Destination != null && TargetDetector.FoundInvalidEnemy(distanceToTargetLocation + 1, _mover.MovementPackage.Destination.MoveDirection,
            _player.transform.position);
    } 
    
    private bool InSafetyRedirectRange() => Vector2.Distance(_player.transform.position, _mover.MovementPackage.Destination.TargetLocation) < _safetyCheckRange;
}