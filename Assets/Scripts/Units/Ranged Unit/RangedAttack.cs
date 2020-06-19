using System;
using System.Collections;
using UnityEngine;

public class RangedAttack : MonoBehaviour, IUnitAttack, IPathRequester
{
    public Transform Mover { get; private set; }
    public event Action<PathRequest> PathRequested;
    public event Action PathCompleted;
    public event Action OnAttackFinished;
    public event Action OnAttackStart;

    [SerializeField] private RangedUnitProjectile _projectilePrefab;
    [SerializeField] private float _shotSpeed = 5f;

    private Player _player;
    private bool _attacked;
    private IUnitEnemy _unitEnemy;
    private PathValues _pathValues;

    private void Start()
    {
        _player = FindObjectOfType<Player>();
        _unitEnemy = GetComponent<IUnitEnemy>();
        Mover = _unitEnemy.Transform;
        _pathValues = new PathValues(4f, 0f, .5f, PathType.Attack);
    }
    
    public IEnumerator Attack()
    {
        PathRequested?.Invoke(new PathRequest(this, _pathValues, FindPath, OnAttackPathArrivalCallback));

        yield break;
    }

    private Vector2[] FindPath()
    {
        OnAttackStart?.Invoke();

        var randomLocation = SpawnHelper.Instance.ValidPointOnScreenXDistanceFromTarget(_player.transform.position, 20f);

        var path = Pathfinder.Path(_unitEnemy, randomLocation);

        while (path == null)
        {
            randomLocation = SpawnHelper.Instance.ValidPointOnScreenXDistanceFromTarget(_player.transform.position, 10f);

            path = Pathfinder.Path(_unitEnemy, randomLocation);
        }

        return path;
    }

    private void OnAttackPathArrivalCallback(bool status)
    {
        if (!status)
        {
            PathCompleted?.Invoke();
            OnAttackFinished?.Invoke();
            return;
        }
        
        StartCoroutine(PerformAttack());
    }

    private IEnumerator PerformAttack()
    {
        var shotDirection = (_player.transform.position - transform.position).normalized;
        
        var projectile = _projectilePrefab.Get<RangedUnitProjectile>(null, transform.position, Quaternion.identity);
        
        projectile.Launch(shotDirection * _shotSpeed);
        
        yield return new WaitForSeconds(1f);
        
        OnAttackFinished?.Invoke();
        PathCompleted?.Invoke();
    }
}