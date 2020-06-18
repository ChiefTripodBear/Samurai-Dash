using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RangedUnitManager : UnitManager
{
    [SerializeField] private float _attackDelayBetweenAttackers = 3f;
    [SerializeField] private int _minAttackerCount;
    [SerializeField] private int _maxAttackerCount;
    [SerializeField] private float _timeUntilAttack = 5f;
    private int _attackFinishedCount;
    private int _currentAttackerCount;
    private int _attackCountDifference;
    private bool _performingAttacks;

    private float _attackTimer;
    private void Start()
    {
        _attackCountDifference = _maxAttackerCount - _minAttackerCount;
    }

    protected override void OnRegisterUnit(IUnitEnemy unitEnemy)
    {
        unitEnemy.UnitAttack.OnAttackFinished += IncreaseAttacksFinished;
        SetAttackingParameters();
    }
    
    protected override void OnRemoveUnit(IUnitEnemy unitEnemy)
    {
        unitEnemy.UnitAttack.OnAttackFinished -= IncreaseAttacksFinished;
    }

    private void IncreaseAttacksFinished()
    {
        _attackFinishedCount++;

        if (_attackFinishedCount >= _currentAttackerCount)
        {
            _performingAttacks = false;
            Debug.Log("Redoing attacks");
        }
    }

    private void SetAttackingParameters()
    {
        _minAttackerCount = _minAttackerCount > Units.Count ? Units.Count : _minAttackerCount;

        _maxAttackerCount = _maxAttackerCount == Units.Count ? _minAttackerCount : _maxAttackerCount;

        _maxAttackerCount = _minAttackerCount + _attackCountDifference <= Units.Count
            ? _minAttackerCount + _attackCountDifference
            : Units.Count;

        _currentAttackerCount = Random.Range(_minAttackerCount, _maxAttackerCount);
        _attackFinishedCount = _currentAttackerCount;
    }

    private void Update()
    {
        if (Units.Count <= 0 || _performingAttacks) return;

        _attackTimer += Time.deltaTime;
        
        if (_attackFinishedCount < _currentAttackerCount || _attackTimer < _timeUntilAttack) return;

        _attackTimer = 0f;
        SetAttackingParameters();
        _attackFinishedCount = 0;
            
        var attackers = Units.Take(_currentAttackerCount).ToList();

        StartCoroutine(PerformAttacksWithDelay(attackers));
    }

    private IEnumerator PerformAttacksWithDelay(List<IUnitEnemy> attackers)
    {
        _performingAttacks = true;
        
        attackers.Shuffle();

        foreach (var attacker in attackers.ToList())
            attacker.KillHandler.OnDeath += () =>
            {
                attackers.Remove(attacker);
                IncreaseAttacksFinished();
            };

        for (var i = 0; i < attackers.Count; i++)
        {
            StartCoroutine(attackers[i].UnitAttack.Attack());
            yield return new WaitForSeconds(i == attackers.Count - 1 ? 0 : _attackDelayBetweenAttackers);
        }
    }
}