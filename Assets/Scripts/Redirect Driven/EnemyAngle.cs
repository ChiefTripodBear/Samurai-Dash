using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyAngle : MonoBehaviour
{
    public static event Action<EnemyAngle> OnKillPointReached;
    [SerializeField] private GameObject _deathVFX;
    [SerializeField] private float _killPointDistance;
    [SerializeField] private float _projectionDistance;
    [SerializeField] private float _angle;
    [SerializeField] private bool _setAngle;

    private float _oppositeAngle;
    private Vector2 _rearPointRelative;
    private Vector2 _forwardPointRelative;
    private Vector2 _killVector;
    private Vector2? _killPoint;
    private Vector2 _intersectionPoint;

    public Vector2 RearPointRelative => _rearPointRelative;
    public Vector2 ForwardPointRelative => _forwardPointRelative;
    public Vector2 KillVector => _killVector;
    public Vector2? KillPoint => _killPoint;
    public Vector2 IntersectionPoint => _intersectionPoint;
    public float Angle => _angle;

    public event Action OnDeath;

    private RedirectPlayer _player;
    private bool _alreadyRegisteredToKillQueue;
    
    private Vector2 _pointOne;
    public Vector2 GetPointOne => _pointOne;

    private Vector2 _pointTwo;
    public Vector2 GetPointTwo => _pointTwo;
    public EnemyMover Mover { get; private set; }

    private void Awake()
    {
        Mover = GetComponent<EnemyMover>();
        _player = FindObjectOfType<RedirectPlayer>();
        
        if (!_setAngle)
            _angle = Random.Range(0, 360);

        _oppositeAngle = _angle - 180 < 0 ? 360 - (180 - _angle) : _angle - 180;
    }

    private void OnDestroy()
    {
        GetComponent<EnemyMover>().CanMove = false;
        OnDeath?.Invoke();
    }

    private void Update()
    {
        if (_player != null && _killPoint != null)
        {
            var killPointToPlayer = (_killPoint.Value - (Vector2) _player.transform.position).normalized;
            var killPointToThis = (_killPoint.Value - (Vector2) transform.position).normalized;
            var dot = Vector2.Dot(killPointToThis, killPointToPlayer);

            if (!_alreadyRegisteredToKillQueue && dot < 0 && Vector2.Distance(_player.transform.position, _killPoint.Value) > 0.1f)
            {
                _alreadyRegisteredToKillQueue = true;
                OnKillPointReached?.Invoke(this);
            }
        }
        
        _killVector = GetKillVector();
        SetRearAndForwardProjectionsRelativeTo(_player.transform.position);
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

    public void SetKillPoint()
    {
        _killPoint = ((Vector2)transform.position - GetPointClosestTo(_player.transform.position)).normalized * _killPointDistance + (Vector2)transform.position;
    }

    public void SetIntersectionPoint(Vector2 intersection)
    {
        _intersectionPoint = intersection;
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

        if (_killPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_killPoint.Value, .3f);
        }

        if (_intersectionPoint != Vector2.zero)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(_intersectionPoint, .5f);
        }
    }
    
    public void Kill()
    {
        StartCoroutine(KillWithDelay());
    }

    private IEnumerator KillWithDelay()
    {
        OnDeath?.Invoke();
        Mover.CanMove = false;

        var currentColorLerpTime = 0f;

        GetComponent<CircleCollider2D>().enabled = false;
        
        while (true)
        {
            currentColorLerpTime += Time.deltaTime;

            if (currentColorLerpTime >= 1f)
            {
                Instantiate(_deathVFX, transform.position, Quaternion.identity);

                Destroy(gameObject);
            }
            
            GetComponent<SpriteRenderer>().color =
                Color.Lerp(GetComponent<SpriteRenderer>().color, Color.red, currentColorLerpTime / 1.5f);
            yield return null;
        }
    }
}