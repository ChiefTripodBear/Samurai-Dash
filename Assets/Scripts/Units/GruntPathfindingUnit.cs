using UnityEngine;

public class GruntPathfindingUnit : MonoBehaviour, IUnitPathFinder
{
    [SerializeField] private float _moveSpeed;

    public bool CanMoveThroughPath { get; set; } = true;
    public RingPosition CurrentRingPosition => _ringPosition;


    private RingPosition _ringPosition;
    private Vector3 _startingPosition;

    private Vector2[] _path;
    private bool _completedRequest = true;
    private int _pathIndex;
    
    private Pathfinder _pathfinder;
    private IUnitAttack _unitAttack;
    private GruntUnitManager _gruntUnitManager;
    
    private void Awake()
    {
        _gruntUnitManager = FindObjectOfType<GruntUnitManager>();
        _unitAttack = GetComponent<IUnitAttack>();
        _pathfinder = GetComponent<Pathfinder>();

        _unitAttack.OnAttackStart += HandleAttackStart;
        _unitAttack.OnAttackFinished += HandleAttackFinish;
    }

    private void OnDestroy()
    {
        _unitAttack.OnAttackStart -= HandleAttackStart;
        _unitAttack.OnAttackFinished -= HandleAttackFinish;
    }

    private void HandleAttackFinish()
    {
        //TODO: Get new ring position
        CanMoveThroughPath = true;
    }

    private void HandleAttackStart()
    {
        _path = null;
        CanMoveThroughPath = false;
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
        
        if (NewPathNeeded())
        {
            _startingPosition = _ringPosition.transform.position;
            _path = _pathfinder.Path(_ringPosition.transform.position, ref _completedRequest);

            if (_path == null && _ringPosition != null)
            {
                _ringPosition.ResetClaim();
                _ringPosition = _gruntUnitManager.GetNextAvailableRingPosition(_ringPosition);
            }
            
            _pathIndex = 0;
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
}