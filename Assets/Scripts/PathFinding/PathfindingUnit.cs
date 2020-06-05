using System;
using System.Collections;
using UnityEngine;

public class PathfindingUnit : PooledMonoBehaviour
{
    public event Action OnAttackFinished;
    [SerializeField] private float _moveSpeed;

    private NodeGrid _nodeGrid;
    private Node _currentNode;
    public Node CurrentNode => _currentNode;

    private RingPosition _ringPosition;
    public RingPosition RingPosition => _ringPosition;
    public bool CanMove { get; set; } = true;

    private Vector2[] _path;

    private int _pathIndex;
    private Vector3 _startingPosition;

    private bool _completedRequest = true;
    private Pathfinder _pathfinder;
    private RingManager _ringManager;

    private bool _selectedToAttack;
    private Player _playerMover;

    private void Awake()
    {
        _playerMover = FindObjectOfType<Player>();
        _pathfinder = GetComponent<Pathfinder>();
        _nodeGrid = FindObjectOfType<NodeGrid>();
        _ringManager = FindObjectOfType<RingManager>();
    }

    private void Update()
    {
        _currentNode = _nodeGrid.NodeFromWorldPosition(transform.position);

        if (_selectedToAttack || !CanMove) return;
        
        if (NewPathNeeded())
        {
            _startingPosition = _ringPosition.transform.position;
            _path = _pathfinder.Path(_ringPosition.transform.position, ref _completedRequest);

            if (_path == null && _ringPosition != null)
            {
                _ringPosition.ResetClaim();
                _ringPosition = _ringManager.GetNextPosition(_ringPosition);
            }
            
            _pathIndex = 0;
        }

        if (_ringPosition == null)
        {
            SetRingPosition(_ringManager.GetNextPosition(null));
        }

        if(_path != null && _path.Length > 0 &&  !ArrivedAtTarget())
        {
            var currentWayPoint = _path[_pathIndex];

            if (transform.position == (Vector3) currentWayPoint)
            {
                _pathIndex++;

                if (TargetMoved() && _completedRequest)
                {
                    _startingPosition = _ringPosition.transform.position;
                    _path = _pathfinder.Path(_ringPosition.transform.position, ref _completedRequest);
                    _pathIndex = 0;
                }
            }

            transform.position = Vector2.MoveTowards(transform.position, currentWayPoint, _moveSpeed * Time.deltaTime);
        }
    }

    public IEnumerator AttackPlayer()
    {
        _path = null;
        _selectedToAttack = true;
        var directionToPlayer = (_playerMover.transform.position - transform.position).normalized;
        var destination = _playerMover.transform.position + directionToPlayer * 10f;
        while (Vector2.Distance(transform.position, destination) > 0.1f)
        {
            
            transform.position = Vector2.MoveTowards(transform.position, destination, _moveSpeed * Time.deltaTime);
            
            yield return null;
        }

        _ringPosition = _ringManager.GetNextPosition(_ringPosition);
        OnAttackFinished?.Invoke();
        _selectedToAttack = false;
    }

    private bool NewPathNeeded()
    {
        return ArrivedAtTarget() && TargetMoved() && _completedRequest 
               || _path == null && _ringPosition != null && _completedRequest 
               || _path != null && _path.Length <= 0 && _ringPosition != null && _completedRequest;
    }

    private bool TargetMoved()
    {
        return _path != null && Vector2.Distance(_ringPosition.transform.position, _startingPosition) > 2f;
    }

    private bool ArrivedAtTarget()
    {
        return _path != null && _path.Length > 0 && (Vector2)transform.position == _path[_path.Length - 1];
    }
    
    public void SetRingPosition(RingPosition ringPosition)
    {
        ringPosition.ClaimPosition(this);
        _ringPosition = ringPosition;
        _startingPosition = ringPosition.transform.position;
        _path = _pathfinder.Path(_ringPosition.transform.position, ref _completedRequest);
    }

    private void OnDrawGizmos()
    {
        if (_selectedToAttack)
        {
            Gizmos.DrawSphere(transform.position, .5f);
        }
        // if (_path != null)
        // {
        //     Gizmos.color = Color.green;
        //
        //     for (var i = 1; i < _path.Length; i++)
        //     {
        //         Gizmos.DrawWireCube(_path[i - 1], Vector2.one);
        //         Gizmos.DrawLine(_path[i - 1], _path[i]);
        //     }
        // }
    }
}
