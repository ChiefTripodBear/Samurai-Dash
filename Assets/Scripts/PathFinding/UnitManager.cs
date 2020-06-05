using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitManager : MonoBehaviour
{
    [SerializeField] private int _minAttackers;
    [SerializeField] private int _maxAttackers;
    [SerializeField] private float _minAttackTime;
    [SerializeField] private float _maxAttackTime;
    private List<PathfindingUnit> _units = new List<PathfindingUnit>();
    private RingManager _ringManager;

    private float _attackTimer;
    private float _timeUntilAttack;
    private int _attackerCount;
    private int _attacksThatHaveBeenFinished;

    private void Awake()
    {
        UnitSpawner.OnUnitSpawned += AddUnit;
        
        _ringManager = FindObjectOfType<RingManager>();
    }

    private void OnDestroy()
    {
        UnitSpawner.OnUnitSpawned -= AddUnit;
    }

    private void AddUnit(PathfindingUnit unit)
    {
        _units.Add(unit);
    }

    private void Start()
    {
        _timeUntilAttack = Random.Range(_minAttackTime, _maxAttackTime);
        _attackerCount = Random.Range(_minAttackers, _maxAttackers);
        _attacksThatHaveBeenFinished = _attackerCount;

        if (_units.Count <= 0) return;
        _units.ForEach(t => t.GetComponent<IKillable>().OnDeath += () => _units.Remove(t));
    }

    public bool IsValidNode(Node nodeQuery, PathfindingUnit pathfindingUnitQuery)
    {
        foreach (var unit in _units)
            if (unit != pathfindingUnitQuery && unit.CurrentNode == nodeQuery)
                return false;

        return true;
    }

    private void Update()
    {
        _attackTimer += Time.deltaTime;

        if (_attackTimer >= _timeUntilAttack && _attacksThatHaveBeenFinished >= _attackerCount)
        {
            _attacksThatHaveBeenFinished = 0;
            _attackTimer = 0f;

            _attackerCount = Random.Range(_minAttackers, _maxAttackers);

            var attackers = _units.Where(t => t.RingPosition.RingOrder == 1).Take(_attackerCount).ToList();

            attackers.ForEach(t => t.OnAttackFinished += () => _attacksThatHaveBeenFinished++);
            attackers.ForEach(t => StartCoroutine(t.AttackPlayer()));
        }

        if (_units.Count <= 0) return;
        
        foreach (var unit in _units)
        {
            if(unit.RingPosition != null) continue;

            unit.SetRingPosition(_ringManager.GetNextPosition(null));
        }
    }
}