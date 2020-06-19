using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class UnitEventSpecificMovements : IPathRequester
{
    public bool CanBeFeared => _fearTimer >= _fearCoolDownTime;
    private float _fearCoolDownTime = 5f;

    public event Action<PathRequest> PathRequested;
    public event Action PathCompleted;
    public Transform Mover { get; }
    
    private readonly IUnitEnemy _unitEnemy;
    private readonly Player _player;

    private readonly PathValues _fearPathValues;
    private float _fearTimer;
    private bool _performingFear;

    public UnitEventSpecificMovements(IUnitEnemy unitEnemy)
    {
        _unitEnemy = unitEnemy;
        Mover = unitEnemy.Transform;
        _player = Object.FindObjectOfType<Player>();
        _fearPathValues = new PathValues(2f, 0.25f, 0.4f, PathType.EventBased);
        _fearTimer = _fearCoolDownTime;
    }
    
    public void PerformFear()
    {
        _performingFear = true;
        PathRequested?.Invoke(new PathRequest(this, _fearPathValues, FindBestTargetPosition, OnPathComplete));
    }

    private void OnPathComplete(bool status)
    {
        _performingFear = false;
        PathCompleted?.Invoke();
    }

    public void TickFearTimer()
    {
        if (_performingFear)
        {
            _fearTimer = 0f;
            return;
        }
        
        _fearTimer += Time.deltaTime;
    }

    private Vector2? FindBestTargetPosition()
    {
        var fearDirection = (_unitEnemy.Transform.position - _player.transform.position).normalized;
        var targetPosition = _unitEnemy.Transform.position + fearDirection * 2f;

        targetPosition =
            BoundaryHelper.AdjustDirectionToFindValidPosition(_unitEnemy.Transform.position, 2f, targetPosition);
        return targetPosition;
    }
}