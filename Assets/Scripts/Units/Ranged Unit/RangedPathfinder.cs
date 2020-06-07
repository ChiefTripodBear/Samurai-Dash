﻿using UnityEngine;

public class RangedPathfinder : MonoBehaviour, IUnitPathFinder
{
    [SerializeField] private float _moveSpeed = 3f;
    private RingPosition _ringPosition;
    private Vector2 _startingPosition;
    public bool CanMoveThroughPath { get; set; } = true;
    public RingPosition CurrentRingPosition => _ringPosition;

    private Pathfinder _pathfinder;
    private bool _completedRequest;
    private Vector2[] _path;
    private int _pathIndex;
    private bool _arrived;
    private IUnitAttack _unitAttack;
    private RangedUnitManager _rangedUnitManager;

    private void Awake()
    {
        _unitAttack = GetComponent<IUnitAttack>();
        _pathfinder = FindObjectOfType<Pathfinder>();
        _rangedUnitManager = FindObjectOfType<RangedUnitManager>();
        _unitAttack.OnAttackFinished += HandleFinishedAttack;
    }

    private void OnDestroy()
    {
        _unitAttack.OnAttackFinished -= HandleFinishedAttack;
    }

    private void HandleFinishedAttack()
    {
        _ringPosition = _rangedUnitManager.GetNextAvailableRingPosition(_ringPosition);

        _path = _pathfinder.Path(_ringPosition.transform.position, ref _completedRequest);

        CanMoveThroughPath = true;
    }

    public void SetRingPosition(RingPosition ringPosition)
    {
        _ringPosition = ringPosition;

        if (_ringPosition == null) return;
        
        _ringPosition.ClaimPosition(this);
        _startingPosition = _ringPosition.transform.position;
        _path = _pathfinder.Path(_ringPosition.transform.position, ref _completedRequest);
    }

    public void Tick()
    {
        if (!CanMoveThroughPath) return;

        if(_path != null && _path.Length > 0)
        {
            var currentWayPoint = _path[_pathIndex];

            if (transform.position == (Vector3) currentWayPoint)
            {
                _pathIndex++;

                if (_pathIndex >= _path.Length)
                {
                    _pathIndex = 0;
                    CanMoveThroughPath = false;
                    StartCoroutine(_unitAttack.Attack());
                    return;
                }
            }

            transform.position = Vector2.MoveTowards(transform.position, currentWayPoint, _moveSpeed * Time.deltaTime);
        }
    }
}