using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMover : MonoBehaviour
{
    [SerializeField] private float _minSpeed;
    [SerializeField] private float _maxSpeed;

    private float _currentMoveSpeed;

    private Player _player;
    private Vector3? _target;
    public bool CanMove { get; set; } = true;

    private void Awake()
    {
        _player = FindObjectOfType<Player>();
        _currentMoveSpeed = Random.Range(_minSpeed, _maxSpeed);
    }

    private void Start()
    {
        _player.PlayerMover.MoveThroughChainStatusChanged += SetTargetToSpecificLocation;
    }

    private void SetTargetToSpecificLocation(bool targetIsSpecificLocation)
    {
        if (targetIsSpecificLocation)
        {
            _target = (Vector2) _player.transform.position;
        }
        else
        {
            _target = Vector2.zero;
            
            if(this != null)
                StartCoroutine(WaitUntilRedirectingToPlayersCurrentPosition());
        }
    }

    private IEnumerator WaitUntilRedirectingToPlayersCurrentPosition()
    {
        yield return new WaitForSeconds(.5f);
        
        if (_target == Vector2.zero)
            _target = null;
    }

    private void Update()
    {
        if (_player != null && CanMove)
        {
            transform.position = Vector2.MoveTowards(transform.position, _target ?? _player.transform.position,
                _currentMoveSpeed * Time.deltaTime);
        }
    }
}

public class DashChargeMonitor : MonoBehaviour
{
    [SerializeField] private int _maxCharges;

    private int _currentCharges;

    private void Awake()
    {
        _currentCharges = _maxCharges;
    }

    public void AddCharge()
    {
        _currentCharges = _currentCharges >= _maxCharges ? _currentCharges : _currentCharges += 2;
    }

    public void RemoveCharge()
    {
        _currentCharges = _currentCharges <= 0 ? 0 : _currentCharges -= 1;
    }
}