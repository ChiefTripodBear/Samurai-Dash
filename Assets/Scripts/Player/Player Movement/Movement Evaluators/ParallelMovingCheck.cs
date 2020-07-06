using System;
using System.Collections.Generic;
using UnityEngine;

public class ParallelMovingCheck
{
    public static event Action<int> OnInvalidEnemyCollision;
    private readonly Transform _mover;
    private readonly PlayerMover _playerMover;

    private readonly LayerMask _enemyMask;
    private readonly Collider2D[] _enemyResults = new Collider2D[10];
    
    private readonly HashSet<IUnit> _invalidCollisions = new HashSet<IUnit>();
    private readonly HashSet<IUnit> _validCollisions = new HashSet<IUnit>();
    
    public ParallelMovingCheck(Transform mover, PlayerMover playerMover)
    {
        _mover = mover;
        _playerMover = playerMover;
        _enemyMask = LayerMask.GetMask("Enemy");
        PlayerMover.OnArrival += ClearInvalidUnits;
        PlayerMover.RequestedNewPackage += ClearInvalidUnits;
    }

    private void ClearInvalidUnits()
    {
        _invalidCollisions.Clear();
        _validCollisions.Clear();
    }

    public void Tick()
    {
        if (!_playerMover.CurrentMoveDirection.HasValue) return;
        
        var size = Physics2D.OverlapCircleNonAlloc(_mover.transform.position, .5f, _enemyResults, _enemyMask);

        if (size <= 0) return;
        
        for (var i = 0; i < size; i++)
        {
            var unit = _enemyResults[i].GetComponent<IUnit>();
            
            if(unit == null || unit.KillHandler.KillPoint.HasValue || _validCollisions.Contains(unit)) continue;

            if (!TargetDetector.DotProductSuccess(unit, _playerMover.CurrentMoveDirection.Value, 0.99f))
            {
                if (!_invalidCollisions.Contains(unit))
                {
                    _invalidCollisions.Add(unit);
                    OnInvalidEnemyCollision?.Invoke(3);
                }
                continue;
            }

            _validCollisions.Add(unit);
            unit.KillHandler.SetKillPoint();
        }
    }
}