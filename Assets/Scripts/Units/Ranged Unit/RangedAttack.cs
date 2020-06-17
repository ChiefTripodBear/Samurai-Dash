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

    private Vector2? _currentDestination;
    private EnemyUnitMover _enemyMover;
    private bool _attacked;

    private void Start()
    {
        _player = FindObjectOfType<Player>();
        var unit  = GetComponent<EnemyUnit>();

        _enemyMover = unit.EnemyUnitMover;
        _enemyMover.AtDestination += SetDestination;
    }

    private void OnDestroy()
    {
        if (_enemyMover == null) return;
        
        _enemyMover.AtDestination -= SetDestination;
    }

    private void SetDestination(Vector2 destination)
    {
        _currentDestination = destination;
    }

    public IEnumerator Attack()
    {
        _currentDestination = null;
        OnAttackStart?.Invoke();
        _attacked = false;

        while (true)
        {
            while (!_currentDestination.HasValue)
            {
                yield return null;
            }

            if (Vector2.Distance(transform.position, _currentDestination.Value) < 0.5f && !_attacked)
            {
                _attacked = true;
                var shotDirection = (_player.transform.position - transform.position).normalized;

                var projectile = _projectilePrefab.Get<RangedUnitProjectile>(null, transform.position, Quaternion.identity);

                projectile.Launch(shotDirection * _shotSpeed);
                OnAttackFinished?.Invoke();
                yield break;
            }

            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        if (_currentDestination != null) Gizmos.DrawWireSphere(_currentDestination.Value, 1f);
    }
}