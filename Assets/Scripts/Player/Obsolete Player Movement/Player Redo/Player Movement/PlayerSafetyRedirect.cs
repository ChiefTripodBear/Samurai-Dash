using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSafetyRedirect
{
    public static event Action InDanger;
    public bool SafetyRedirectAllowed { get; set; }
    private readonly PlayerMovementController _playerMovementController;
    private readonly PlayerMover _mover;
    private readonly Player _player;
    private float _safetyCheckRange = 4f;

    private float _safetyRedirectMoveSpeed = 10f;
    private readonly LayerMask _enemyLayer;
    private bool _inDanger;

    public PlayerSafetyRedirect(PlayerMovementController playerMovementController, PlayerMover mover, Player player)
    {
        _playerMovementController = playerMovementController;
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
                _playerMovementController.SafetyRedirectMove();
                _inDanger = false;
            }
        }
        else
        {
            Time.timeScale = 1f;
            if (_player.PlayerCollider2D.enabled)
            {
                if (InDangerAtTargetLocation())
                {
                    _inDanger = true;
                }
            }
            else
            {
                _inDanger = false;
            }
        }
    }
    
    private bool InDangerAtTargetLocation()
    {
        if (_mover.CurrentPlan == null || _mover.CurrentMoveDirection == null)
        {
            return false;
        }

        var hit = Physics2D.Raycast(_player.transform.position, _mover.CurrentPlan.MoveDirection, 5f, _enemyLayer);

        if (!hit) return false;
        
        var unit = hit.collider.GetComponent<IUnit>();

        if (unit == null)
        {
            return false;
        }

        return _mover.CurrentPlan.TargetUnit != unit;
    }

    public void DrawSafetyPosition()
    {
    }
}