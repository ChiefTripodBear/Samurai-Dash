using UnityEngine;

public class RingPosition : MonoBehaviour
{
    [SerializeField] private bool _randomizeSpeed;
    [SerializeField] private float _moveSpeed;
    private PathfindingUnit _pathfindingUnit;
    private int _ringOrder;
    private Vector2 _startingPosition;
    private float _startingAngle;
    private float _radius;

    private Player _player;
    private Vector2 _oppositePoint;
    public Vector2 OppositePoint => _oppositePoint;

    public bool Claimed => _pathfindingUnit != null;
    public int RingOrder => _ringOrder;

    private void Awake()
    {
        _player = FindObjectOfType<Player>();
    }

    private void Start()
    {
        if(_randomizeSpeed)
            _moveSpeed = Random.Range(-.5f, .5f);
    }

    public void ClaimPosition(PathfindingUnit pathfindingUnit)
    {
        _pathfindingUnit = pathfindingUnit;
    }

    private void Update()
    {
        _startingAngle += Time.deltaTime * _moveSpeed;
        _startingPosition = new Vector2(Mathf.Sin(_startingAngle), Mathf.Cos(_startingAngle)) * _radius + (Vector2)transform.parent.position;
        transform.position = _startingPosition;
    }

    public void Initialize(float angle, float radius, int ringOrder)
    {
        _startingAngle = angle;
        _radius = radius;
        _ringOrder = ringOrder;

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
        _pathfindingUnit = null;
    }
}