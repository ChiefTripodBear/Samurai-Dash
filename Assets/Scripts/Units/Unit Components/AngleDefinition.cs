using UnityEngine;

[RequireComponent(typeof(UnitAngleVisualizer))]
public class AngleDefinition : MonoBehaviour
{
    [SerializeField] private float _projectionDistance;
    [SerializeField] private float _angle;
    [SerializeField] private bool _setAngle;

    private float _oppositeAngle;
    private Vector2 _rearPointRelative;
    private Vector2 _forwardPointRelative;
    private Vector2 _killVector;
    private Vector2 _intersectionPoint;

    public Vector2 RearPointRelative => _rearPointRelative;
    public Vector2 ForwardPointRelative => _forwardPointRelative;
    public Vector2 KillVector => _killVector;
    public Vector2 IntersectionPoint => _intersectionPoint;
    public float Angle => _angle;
    
    private Player _player;
    private bool _alreadyRegisteredToKillQueue;
    
    private Vector2 _pointOne;
    public Vector2 GetPointOne => _pointOne;

    private Vector2 _pointTwo;
    private float _distanceFromUnit;
    private Vector2 _directionFromUnitToIntersectPoint;
    public Vector2 GetPointTwo => _pointTwo;

    private void Awake()
    {
        _player = FindObjectOfType<Player>();
        
        if (!_setAngle)
            _angle = Random.Range(0, 360);

        _oppositeAngle = _angle - 180 < 0 ? 360 - (180 - _angle) : _angle - 180;
    }
    
    private void Update()
    {
        _killVector = GetKillVector();
        SetRearAndForwardProjectionsRelativeTo(_player.transform.position);

        if (_intersectionPoint != Vector2.zero)
        {
            _intersectionPoint = (Vector2) transform.position + _directionFromUnitToIntersectPoint * _distanceFromUnit;
        }
    }

    private Vector2 GetKillVector()
    {
        var closestPointToPlayer = GetPointClosestTo(_player.transform.position);

        return ((Vector2)transform.position - closestPointToPlayer).normalized;
    }

    public Vector2 GetPointClosestTo(Vector2 position)
    {
        _pointOne = PointOne();
        _pointTwo = PointTwo();

        var distanceToPointOne = Vector2.Distance(position, _pointOne);
        var distanceToPointTwo = Vector2.Distance(position, _pointTwo);

        return distanceToPointOne < distanceToPointTwo ? _pointOne : _pointTwo;
    }
    
    private void SetRearAndForwardProjectionsRelativeTo(Vector2 position)
    {
        var closestPoint = GetPointClosestTo(position);
        var forwardDirection = (closestPoint - (Vector2) transform.position).normalized;
        _forwardPointRelative = (Vector2) transform.position + forwardDirection * _projectionDistance;

        _rearPointRelative = (Vector2) transform.position + -forwardDirection * _projectionDistance;
    }
    
    private Vector2 PointTwo()
    {
        return new Vector2(Mathf.Sin(_oppositeAngle * Mathf.Deg2Rad), Mathf.Cos(_oppositeAngle *  Mathf.Deg2Rad)) + (Vector2) transform.position;
    }

    private Vector2 PointOne()
    {
        return new Vector2(Mathf.Sin(_angle * Mathf.Deg2Rad), Mathf.Cos(_angle * Mathf.Deg2Rad)) + (Vector2) transform.position;
    }
    
    public void SetIntersectionPoint(Vector2 intersection)
    {
        _intersectionPoint = intersection;
        _distanceFromUnit = Vector2.Distance(transform.position, _intersectionPoint);
        _directionFromUnitToIntersectPoint = (_intersectionPoint - (Vector2) transform.position).normalized;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(_rearPointRelative, _forwardPointRelative);
        Gizmos.DrawWireSphere(_forwardPointRelative, 1f);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_rearPointRelative, 1f);
        if (_player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(GetPointClosestTo(_player.transform.position), .3f);
        }

        if (_intersectionPoint != Vector2.zero)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(_intersectionPoint, .5f);
        }
    }
}