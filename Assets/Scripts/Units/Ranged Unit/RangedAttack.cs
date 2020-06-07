using System;
using System.Collections;
using UnityEngine;

public class RangedAttack : MonoBehaviour, IUnitAttack
{
    [SerializeField] private RangedUnitProjectile _projectilePrefab;
    [SerializeField] private float _shotSpeed = 5f;
    public event Action OnAttackFinished;
    public event Action OnAttackStart;

    private Player _player;

    private void Awake()
    {
        _player = FindObjectOfType<Player>();
    }

    public IEnumerator Attack()
    {
        OnAttackStart?.Invoke();

        var shotDirection = (_player.transform.position - transform.position).normalized;

        var projectile = _projectilePrefab.Get<RangedUnitProjectile>(null, transform.position, Quaternion.identity);

        projectile.Launch(shotDirection * _shotSpeed);
        OnAttackFinished?.Invoke();
        yield break;
    }
}