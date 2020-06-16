using UnityEngine;

public class RingPosition : MonoBehaviour, IWaypoint
{
    public Transform Transform => transform;
    public bool IsClaimed => _unitEnemyMover != null;

    [SerializeField] private bool _randomizeSpeed;
    [SerializeField] private float _moveSpeed;

    private Vector2 _startingPosition;
    private float _currentAngle;
    private float _radius;
    private Vector2 _oppositePoint;
    public Vector2 OppositePoint => _oppositePoint;

    private EnemyUnitMover _unitEnemyMover;
    private Player _player;
    private int _ringOrder;

    private void Awake()
    {
        _player = FindObjectOfType<Player>();
    }

    private void Start()
    {
        if(_randomizeSpeed)
            _moveSpeed = Random.Range(-.5f, .5f);
    }
    
    public void Claim(EnemyUnitMover enemyUnitMover)
    {
        _unitEnemyMover = enemyUnitMover;
    }
    
    private void Update()
    {
        _currentAngle += Time.deltaTime * _moveSpeed;
        _startingPosition = new Vector2(Mathf.Sin(_currentAngle), Mathf.Cos(_currentAngle)) * _radius + (Vector2)transform.parent.position;
        transform.position = _startingPosition;
    }

    public void Initialize(float angle, float radius, int ringOrder)
    {
        _ringOrder = ringOrder;
        _currentAngle = angle;
        _radius = radius;

        _player = FindObjectOfType<Player>();
        _startingPosition = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * _radius + (Vector2)transform.parent.position;
        transform.position = _startingPosition;
        
        var directionToCenter = (_player.transform.position - transform.position).normalized;
        var angleToCenter = Mathf.Atan2(directionToCenter.y, directionToCenter.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angleToCenter - 90);
        _oppositePoint = transform.position + transform.up * (_radius * 2);
    }

    public void ResetClaim()
    {
        _unitEnemyMover = null;
    }
}