using System;
using System.Collections;
using UnityEngine;

public class RangedAttack : MonoBehaviour, IUnitAttack
{
    public event Action OnAttackFinished;
    public event Action OnAttackStart;

    [SerializeField] private RangedUnitProjectile _projectilePrefab;
    [SerializeField] private float _shotSpeed = 5f;

    private Player _player;
    private bool _attacked;
    private IUnitEnemy _unitEnemy;

    private void Start()
    {
        _player = FindObjectOfType<Player>();
        _unitEnemy = GetComponent<IUnitEnemy>();
    }
    
    public IEnumerator Attack()
    {
        OnAttackStart?.Invoke();
        
        while (_unitEnemy.EnemyUnitMover.IsCurrentlyMoving)
            yield return null;
        
        var randomLocation = SpawnHelper.Instance.ValidPointOnScreenXDistanceFromTarget(_player.transform.position, 20f);

        var success = _unitEnemy.UnitMovementManager.RequestPathGivenDestination(randomLocation, OnAttackPathArrivalCallback);

        while (!success)
        {
            randomLocation = SpawnHelper.Instance.ValidPointOnScreenXDistanceFromTarget(_player.transform.position, 10f);
            success = _unitEnemy.UnitMovementManager.RequestPathGivenDestination(randomLocation, OnAttackPathArrivalCallback);
        }
    }

    private void OnAttackPathArrivalCallback()
    {
        StartCoroutine(PerformAttack());
    }

    private IEnumerator PerformAttack()
    {
        var shotDirection = (_player.transform.position - transform.position).normalized;
        
        var projectile = _projectilePrefab.Get<RangedUnitProjectile>(null, transform.position, Quaternion.identity);
        
        projectile.Launch(shotDirection * _shotSpeed);
        
        yield return new WaitForSeconds(1f);
        
        OnAttackFinished?.Invoke();
    }
}