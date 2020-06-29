using System;
using UnityEngine;

public class PlayerDashMonitor
{
    public static event Action RemovedChargesFromCollisionWithInvalidEnemy;
    public static event Action<int> OnDashChargesChanged;

    public int CurrentCharges => _currentCharges;
    private int _currentCharges;
    private int _maxCharges = 8;

    private float _rechargeTime = 1f;
    private float _rechargeTimer;

    public PlayerDashMonitor()
    {
        UnitKillHandler.UnitKillPointReached += AddCharge;
        MovementPackage.OnFirstMove += RemoveCharge;
        MovementPackage.OnCollisionWithEnemyWhileMovingThroughIntersection += RemoveCharge;
        _currentCharges = _maxCharges;
    }

    private void RemoveCharge()
    {
        _rechargeTimer = 0f;
        _currentCharges = _currentCharges <= 0 ? 0 : _currentCharges -= 1;
        RemovedChargesFromCollisionWithInvalidEnemy?.Invoke();
        OnDashChargesChanged?.Invoke(_currentCharges);
    }

    private void AddCharge()
    {
        _currentCharges = _currentCharges >= _maxCharges ? _maxCharges : _currentCharges += 2;
        OnDashChargesChanged?.Invoke(_currentCharges);
    }

    public void RefundChargesWhileNotMoving()
    {
        _rechargeTimer += Time.deltaTime;

        if (_rechargeTimer >= _rechargeTime && _currentCharges < _maxCharges)
        {
            var chargesNeedToFull = _maxCharges - _currentCharges;
            
            for (var i = 0; i < chargesNeedToFull; i++)
                AddCharge();
            
            _rechargeTimer = 0f;
        }
    }
}