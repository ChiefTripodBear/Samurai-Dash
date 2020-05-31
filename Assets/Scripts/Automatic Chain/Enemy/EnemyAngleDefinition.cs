using UnityEngine;

public class EnemyAngleDefinition : MonoBehaviour
{
    [SerializeField] private bool _setAngle;
    [SerializeField] private float _angle;
    [SerializeField] private float _projectionDistance = 5f;
    private float _oppositeAngle;
    
    private Vector2 _killLineStart;
    private Vector2 _killLineEnd;

    private Vector2 _killVector;
    public Vector2 KillVector => _killVector;

    private Vector2 _forwardProjectionPoint;
    private Vector2 _rearProjectionPoint;
    private Vector2 _start;
    private Vector2 _end;

    public Vector2 ForwardProjectionPoint => _forwardProjectionPoint;
    public Vector2 RearProjectionPoint => _rearProjectionPoint;

    private LineRenderer _lineRenderer;
    private Player _playerController;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _playerController = FindObjectOfType<Player>();
        
        if(!_setAngle)
            _angle = Random.Range(0, 360);
    }
    
    private void Update()
    {
        _killLineStart = StartKillPoint(_angle);
        _killLineEnd = EndKillPoint(_angle);

        _killVector = GetValidKillVector();

        SetIntersectionProjectionPoints();
    }

    private void SetIntersectionProjectionPoints()
    {
        var direction = (_start - (Vector2) transform.position).normalized;

        _forwardProjectionPoint = direction * _projectionDistance + (Vector2)transform.position;

        _rearProjectionPoint = -direction * _projectionDistance + (Vector2) transform.position;
    }

    public Vector2 GetRearProjectionAtTimeOfIntersection(Vector2 startAtTimeOfIntersection)
    {
        var direction = (startAtTimeOfIntersection - (Vector2) transform.position).normalized;

        return -direction * _projectionDistance + (Vector2) transform.position;
    }
    
    private Vector2 StartKillPoint(float angle)
    {
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad)) + transform.position;;
    }

    private Vector2 EndKillPoint(float angle)
    {
        _oppositeAngle = angle - 180 <= 0 ? 360 - (180 - angle) : angle - 180;
        
        return new Vector3(Mathf.Sin(_oppositeAngle * Mathf.Deg2Rad), Mathf.Cos(_oppositeAngle * Mathf.Deg2Rad)) + transform.position;
    }

    private Vector2 GetValidKillVector()
    {
        if (_playerController == null) return Vector2.zero;
        
        _start = GetKillLineStart(_playerController.transform.position);

        _end = _start == _killLineStart ? _killLineEnd : _killLineStart;

        _lineRenderer.SetPosition(0, _start);
        _lineRenderer.SetPosition(1, _end);

        return (_end - _start).normalized;
    }

    private Vector2 GetKillLineStart(Vector2 atPosition)
    {
        return Vector2.Distance(atPosition, _killLineStart) <
               Vector2.Distance(atPosition, _killLineEnd)
            ? _killLineStart
            : _killLineEnd;
    }

    private void OnDrawGizmos()
    {
        // Gizmos.color = Color.green;
        // Gizmos.DrawWireSphere(_start, .2f);
        //
        // Gizmos.color = Color.magenta;
        // Gizmos.DrawWireSphere(_forwardProjectionPoint, .3f);
        // Gizmos.color = Color.blue;
        // Gizmos.DrawWireSphere(_rearProjectionPoint, .3f);
        //
        // Gizmos.color = Color.yellow;
        // Gizmos.DrawLine(_forwardProjectionPoint, _rearProjectionPoint);
    }
}

