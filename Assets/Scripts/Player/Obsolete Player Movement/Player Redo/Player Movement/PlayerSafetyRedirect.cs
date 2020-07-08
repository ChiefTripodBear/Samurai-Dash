using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSafetyRedirect
{
    public static event Action InDanger;
    public bool SafetyRedirectAllowed { get; set; }
    private PlayerMover _mover;
    private readonly Player _player;
    private float _safetyCheckRange = 4f;

    private float _safetyRedirectMoveSpeed = 10f;
    private LayerMask _enemyLayer;
    private Vector2 _projectedPoint;
    private Vector2 _dangerousPosition;
    private bool _inDanger;

    public PlayerSafetyRedirect(PlayerMover mover, Player player)
    {
        _mover = mover;
        _player = player;
        _enemyLayer = LayerMask.GetMask("Enemy");
    }

    public void Tick()
    {
        if (_inDanger)
        {
            Time.timeScale = 0.1f;
            
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                Time.timeScale = 1f;
                _mover.Reset();
                _inDanger = false;
            }
        }
        else
        {
            if (InDangerAtTargetLocation())
            {
                _inDanger = true;
            }
        }
    }
    
    private bool InDangerAtTargetLocation()
    {
        if (_mover.CurrentPlan == null || _mover.CurrentMoveDirection == null) return false;

        _projectedPoint = (Vector2)_player.transform.position + _mover.CurrentMoveDirection.Value * 1.5f;

        var checkCircle = Physics2D.OverlapCircle(_projectedPoint, 1f, _enemyLayer);

        if (!checkCircle) return false;

        var unit = checkCircle.GetComponent<IUnit>();

        if (unit == null) return false;

        _dangerousPosition = _projectedPoint;
        return _mover.CurrentPlan.TargetUnit != unit;
    }

    public void DrawSafetyPosition()
    {
        Gizmos.DrawWireSphere(_projectedPoint, 1f);
    }
}