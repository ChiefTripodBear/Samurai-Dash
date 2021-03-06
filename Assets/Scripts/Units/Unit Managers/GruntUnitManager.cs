﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GruntUnitManager : UnitManager
{
    [SerializeField] private int _defaultMinAttackers;
    [SerializeField] private int _defaultMaxAttackers;
    [SerializeField] private float _minTimeBetweenAttacks = 5f;
    [SerializeField] private float _maxTimeBetweenAttacks = 10f;
    [SerializeField] private float _attackDelayBetweenAttackers;

    private float _attackTimer;
    private float _timeUntilAttack;
    private int _attackerCount;
    private int _attacksThatHaveBeenFinished;
    private int _attackCountDifference;
    private bool _performingAttacks;

    private int _currentMinAttackers;
    private int _currentMaxAttackers;

    private void Start()
    {
        _attackCountDifference = _defaultMaxAttackers - _defaultMinAttackers;

        _currentMinAttackers = _defaultMinAttackers;
        _currentMaxAttackers = _defaultMaxAttackers;
    }

    private void SetAttackingParameters()
    {
        _timeUntilAttack = Random.Range(_minTimeBetweenAttacks, _maxTimeBetweenAttacks);
        _attacksThatHaveBeenFinished = _attackerCount;
        
        _currentMinAttackers = _defaultMinAttackers > Units.Count ? Units.Count : _defaultMinAttackers;

        _currentMaxAttackers = _defaultMinAttackers == Units.Count ? _defaultMinAttackers : _defaultMaxAttackers;

        _currentMaxAttackers = _defaultMinAttackers + _attackCountDifference <= Units.Count
            ? _defaultMinAttackers + _attackCountDifference
            : Units.Count;

        _attackerCount = Random.Range(_currentMinAttackers, _currentMaxAttackers);
        _attacksThatHaveBeenFinished = _attackerCount;
    }

    protected override void OnRegisterUnit(IUnitEnemy unitEnemy)
    {
        unitEnemy.UnitAttack.OnAttackFinished += IncreaseFinishedAttacks;
        SetAttackingParameters();
    }

    protected override void OnRemoveUnit(IUnitEnemy unitEnemy)
    {
        unitEnemy.UnitAttack.OnAttackFinished -= IncreaseFinishedAttacks;
    }

    private void IncreaseFinishedAttacks()
    {
        _attacksThatHaveBeenFinished++;

        if (_attacksThatHaveBeenFinished >= _attackerCount)
            _performingAttacks = false;
    }
    
    private void Update()
    {
        if (Units.Count <= 0 || _performingAttacks) return;
        
        _attackTimer += Time.deltaTime;

        if (_attackTimer >= _timeUntilAttack && _attacksThatHaveBeenFinished >= _attackerCount)
        {
            _attackTimer = 0f;
            SetAttackingParameters();
            _attacksThatHaveBeenFinished = 0;
            
            var attackers = Units.Take(_attackerCount).ToList();

            StartCoroutine(PerformAttacksWithDelay(attackers));
        }
    }

    private IEnumerator PerformAttacksWithDelay(List<IUnitEnemy> attackers)
    {
        _performingAttacks = true;

        attackers.Shuffle();

        foreach (var attacker in attackers.ToList())
        {
            attacker.KillHandler.OnDeath += () =>
            {
                attackers.Remove(attacker);
                IncreaseFinishedAttacks();
            };
        }
        
        for (var i = 0; i < attackers.Count; i++)
        {
            StartCoroutine(attackers[i].UnitAttack.Attack());
            yield return new WaitForSeconds(i == attackers.Count - 1 ? 0 : _attackDelayBetweenAttackers);
        }
    }
}