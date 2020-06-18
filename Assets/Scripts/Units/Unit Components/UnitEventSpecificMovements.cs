using System.Collections;
using UnityEngine;

public class UnitEventSpecificMovements
{
    private float _fearTime = 1f;
    
    private readonly IUnitEnemy _unitEnemy;
    private readonly Player _player;

    public UnitEventSpecificMovements(IUnitEnemy unitEnemy, Player player)
    {
        _unitEnemy = unitEnemy;
        _player = player;
    }

    public void PerformFear()
    {
        _unitEnemy.MonoBehaviour.StartCoroutine(HandleFearPathing());
    }

    private IEnumerator HandleFearPathing()
    {
        var targetPosition = FindBestTargetPosition();
        
        _unitEnemy.UnitMovementManager.MoveToPoint(targetPosition, true, 1f, .25f);

        yield break;
    }

    private Vector3 FindBestTargetPosition()
    {
        var fearDirection = (_unitEnemy.Transform.position - _player.transform.position).normalized;
        var targetPosition = _unitEnemy.Transform.position + fearDirection * 2f;

        targetPosition =
            BoundaryHelper.AdjustDirectionToFindValidPosition(_unitEnemy.Transform.position, 2f, targetPosition);
        return targetPosition;
    }
}