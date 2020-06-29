using System.Collections.Generic;
using UnityEngine;

public class RingPosition : MonoBehaviour, IWaypoint
{
    public Transform FCostLocation;
    public Transform GCostLocation;
    public Transform HCostLocation;
    public Transform Transform => transform;
    public bool IsClaimed => _unitEnemyMover != null;

    [SerializeField] private float _neighborRadius = 5;
    [SerializeField] private bool _randomizeSpeed;
    [SerializeField] private float _moveSpeed;

    private Vector2 _startingPosition;
    private float _currentAngle;
    private float _radius;
    private Vector2 _oppositePoint;
    public Vector2 OppositePoint => _oppositePoint;

    private IUnitEnemy _unitEnemyMover;
    private Player _player;
    private int _ringOrder;
    
    public RingNode RingNode { get; private set; }
    
    private void Awake()
    {
        _player = FindObjectOfType<Player>();
    }

    private void Start()
    {
        if(_randomizeSpeed)
            _moveSpeed = Random.Range(-.5f, .5f);
    }
    
    public void Claim(IUnitEnemy enemyUnitMover)
    {
        _unitEnemyMover = enemyUnitMover;
    }
    
    private void Update()
    {
        _currentAngle += Time.deltaTime * _moveSpeed;
        _startingPosition = new Vector2(Mathf.Sin(_currentAngle), Mathf.Cos(_currentAngle)) * _radius + (Vector2)transform.parent.position;
        transform.position = _startingPosition;
    }

    public void InitializePositionValues(float angle, float radius, int ringOrder)
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

    public void SetRingNode()
    {
        RingNode = new RingNode(this);
    }
}

public class RingNode : IHeap<RingNode>
{
    private readonly RingPosition _ringPosition;
    public RingPosition RingPosition => _ringPosition;

    private readonly float _neighborRadius;

    public float HCost, GCost;
    public float FCost => HCost + GCost;
    public RingNode ParentRing;
    
    
    public RingNode(RingPosition ringPosition)
    {
        _ringPosition = ringPosition;
    }
    
    public bool IsWalkable()
    {
        if (!NodeGrid.Instance.PositionInsideBounds(_ringPosition.transform.position)) return false;
        return NodeGrid.Instance.NodeFromWorldPosition(_ringPosition.transform.position).IsWalkable;
    }

    public int CompareTo(RingNode other)
    {
        var comparison = FCost.CompareTo(other.FCost);

        if (comparison == 0)
            comparison = HCost.CompareTo(other.HCost);

        return -comparison;
    }

    public int HeapIndex { get; set; }
}