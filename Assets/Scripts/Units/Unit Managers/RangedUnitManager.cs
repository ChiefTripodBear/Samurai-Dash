using System.Collections;
using System.Linq;
using UnityEngine;

public class RangedUnitManager : UnitManager
{
    [SerializeField] private int _minAttackerCount;
    [SerializeField] private int _maxAttackerCount;

    private int _attackFinishedCount;

    private int _currentAttackerCount;
    private int _attackCountDifference;
    private bool _performingAttacks;
    private bool _noValidAttackersFound;

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
    
    protected override void OnRegisterUnit(IUnitEnemy unitEnemy)
    {
        unitEnemy.UnitAttack.OnAttackFinished += IncreaseAttackFinishedCount;
        
        SetAttackingParameters();    
    }

    protected override void OnRemoveUnit(IUnitEnemy unitEnemy)
    {
        unitEnemy.UnitAttack.OnAttackFinished -= IncreaseAttackFinishedCount;
    }

    private void IncreaseAttackFinishedCount()
    {
        _attackFinishedCount++;

        if (_attackFinishedCount >= _currentAttackerCount)
            _performingAttacks = false;
    }
    
    private void Update()
    {
        if (Units.Count <= 0 || _performingAttacks) return;
        
        SelectAttackers();
    }
    
    private void SelectAttackers()
    {
        _performingAttacks = true;
        
        SetAttackingParameters();
        _attackFinishedCount = 0;

        var attackers = Units
            .Where(t => Vector2.Distance(Player.transform.position, t.Transform.position) > 5f)
            .Take(_currentAttackerCount)
            .ToList();

        _noValidAttackersFound = attackers.Count <= 0;

        if (_noValidAttackersFound)
        {
            StartCoroutine(WaitToTryNewAttackers());
            return;
        }

        foreach (var attacker in attackers.ToList())
        {
            attacker.KillHandler.OnDeath += () =>
            {
                attackers.Remove(attacker);
                IncreaseAttackFinishedCount();
            };
        }
        
        attackers.ForEach(t => StartCoroutine(t.UnitAttack.Attack()));
    }

    private IEnumerator WaitToTryNewAttackers()
    {
        yield return new WaitForSeconds(3f);

        _attackFinishedCount = _currentAttackerCount;
        _performingAttacks = false;
    }
}