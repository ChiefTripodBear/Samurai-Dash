using System;
using System.Collections;
using UnityEngine;

public class GruntAttack : MonoBehaviour, IUnitAttack, IPathRequester
{
    [SerializeField] private float _dashAttackSpeed = 6f;
    [SerializeField] private float _waitTimeAfterDash = 1f;
    
    public Transform Mover { get; private set; }
    public event Action<PathRequest> PathRequested;
    public event Action PathCompleted;

    public event Action OnAttackFinished;
    public event Action OnAttackStart;
    
    private Player _player;

    private Vector2 _attackDestination;
    private SpriteRenderer _spriteRenderer;
    private Color _defaultColor;
    private PathValues _pathValues;

    private void Start()
    {
        Mover = GetComponent<GruntEnemyUnit>().transform;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _defaultColor = _spriteRenderer.color;
        _player = FindObjectOfType<Player>();
        
        _pathValues = new PathValues(_dashAttackSpeed, 0f, _waitTimeAfterDash, PathType.Attack);
    }

    public IEnumerator Attack()
    {
        PathRequested?.Invoke(new PathRequest(this, _pathValues, FindAttackDestination, OnPathFinished));
        yield break;
    }

    private Vector2? FindAttackDestination()
    {
        OnAttackStart?.Invoke();
        _spriteRenderer.color = Color.yellow;

        var directionToPlayer = (_player.transform.position - transform.position).normalized;
        var projectedDestination = _player.transform.position + directionToPlayer * 10f;
        _attackDestination = BoundaryHelper.PointBeforeCollisionWithBoundaryWithBuffer(transform.position, projectedDestination, 2f);

        return _attackDestination;
    }
    
    private void OnPathFinished(bool status)
    {
        _spriteRenderer.color = _defaultColor;

        OnAttackFinished?.Invoke();
        PathCompleted?.Invoke();
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

public struct PathValues
{
    public float MoveSpeed { get; }
    public float PrePathWaitTime { get; }
    public float PostPathWaitTime { get; }
    public PathType PathType { get; }

    public PathValues(float moveSpeed, float prePathWaitTime, float postPathWaitTime, PathType pathType)
    {
        MoveSpeed = moveSpeed;
        PrePathWaitTime = prePathWaitTime;
        PostPathWaitTime = postPathWaitTime;
        PathType = pathType;
    }
}