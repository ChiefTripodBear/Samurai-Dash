using System;
using System.Collections;
using UnityEngine;

public class GruntAttack : MonoBehaviour, IUnitAttack
{
    [SerializeField] private float _dashAttackSpeed = 6f;
    [SerializeField] private float _waitTimeAfterDash = 1f;
    public event Action OnAttackFinished;
    public event Action OnAttackStart;
    
    private Player _player;

    private Vector2 _attackDestination;
    private SpriteRenderer _spriteRenderer;
    private Color _defaultColor;
    private UnitMovementManager _movementManager;
    
    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _defaultColor = _spriteRenderer.color;
        _player = FindObjectOfType<Player>();
        _movementManager = GetComponent<IUnitEnemy>().UnitMovementManager;
    }

    public IEnumerator Attack()
    {
        _spriteRenderer.color = Color.yellow;
        OnAttackStart?.Invoke();
        
        var directionToPlayer = (_player.transform.position - transform.position).normalized;
        var projectedDestination = _player.transform.position + directionToPlayer * 10f;
        _attackDestination = BoundaryHelper.PointBeforeCollisionWithBoundaryWithBuffer(transform.position, projectedDestination, 2f);
        
        _movementManager.MoveToPoint(_attackDestination, true, _waitTimeAfterDash, 0f, 6f, OnAttackPathArrival);

        yield break;
    }

    private void OnAttackPathArrival()
    {
        _spriteRenderer.color = _defaultColor;

        OnAttackFinished?.Invoke();
    }

    private void OnDrawGizmos()
    {
        if (_attackDestination != Vector2.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_attackDestination, 1f);
        }
    }
}