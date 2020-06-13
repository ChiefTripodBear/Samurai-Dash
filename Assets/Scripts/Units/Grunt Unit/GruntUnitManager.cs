using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GruntUnitManager : UnitManager<GruntEnemyUnit>
{
    [SerializeField] private int _minAttackers;
    [SerializeField] private int _maxAttackers;
    [SerializeField] private float _minAttackTime;
    [SerializeField] private float _maxAttackTime;
    
    private float _attackTimer;
    private float _timeUntilAttack;
    private int _attackerCount;
    private int _attacksThatHaveBeenFinished;

    private void Start()
    {
        _timeUntilAttack = Random.Range(_minAttackTime, _maxAttackTime);
        _attackerCount = Random.Range(_minAttackers, _maxAttackers);
        _attacksThatHaveBeenFinished = _attackerCount;

        Units.ForEach(t => t.GetComponent<IKillable>().OnDeath += () => Units.Remove(t));
    }

    protected override void Update()
    {
        base.Update();
        
        _attackTimer += Time.deltaTime;

        if (_attackTimer >= _timeUntilAttack && _attacksThatHaveBeenFinished >= _attackerCount)
        {
            _attacksThatHaveBeenFinished = 0;
            _attackTimer = 0f;

            _attackerCount = Random.Range(_minAttackers, _maxAttackers);

            var attackers = Units.Take(_attackerCount).ToList();

            attackers.ForEach(t => t.UnitAttack.OnAttackFinished += () => _attacksThatHaveBeenFinished++);
            attackers.ForEach(t => StartCoroutine(t.UnitAttack.Attack()));
        }
    }
}