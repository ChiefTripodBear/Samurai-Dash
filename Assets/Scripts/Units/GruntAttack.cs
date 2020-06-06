using System;
using System.Collections;
using UnityEngine;

public class GruntAttack : MonoBehaviour, IUnitAttack
{
    [SerializeField] private float _dashAttackSpeed = 6f;
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
        
        var directionToPlayer = (_player.transform.position - transform.position).normalized;
        var destination = _player.transform.position + directionToPlayer * 10f;
        while (Vector2.Distance(transform.position, destination) > 0.1f)
        {
            
            transform.position = Vector2.MoveTowards(transform.position, destination, _dashAttackSpeed * Time.deltaTime);
            
            yield return null;
        }

        OnAttackFinished?.Invoke();
    }
}