using System;
using System.Collections;
using UnityEngine;

public class UnitKillHandler : MonoBehaviour
{
    private const float KillPointDistance = 0.55f;
    public static event Action OnAnyEnemyDeath;
    public event Action OnDeath;
    public static event Action UnitKillPointReached;

    [SerializeField] private GameObject _deathVFX;

    private bool _dead;
    private Vector2? _killPoint;
    public Vector2? KillPoint => _killPoint;

    private AngleDefinition _unitAngle;
    private Player _player;
    private Color _defaultColor;
    
    private void Awake()
    {
        _player = FindObjectOfType<Player>();
        _unitAngle = GetComponent<AngleDefinition>();
        _defaultColor = GetComponent<SpriteRenderer>().color;
    }

    public void SetKillPoint()
    {
        _killPoint = ((Vector2)transform.position - _unitAngle.GetPointClosestTo(_player.transform.position)).normalized * KillPointDistance + (Vector2)transform.position;
    }

    public Vector2 GetFauxKillPoint()
    {
        return ((Vector2)transform.position - _unitAngle.GetPointClosestTo(_player.transform.position)).normalized * KillPointDistance + (Vector2)transform.position;
    }

    private void Update()
    {
        if (_player != null && _killPoint != null)
        {
            var killPointToPlayer = (_killPoint.Value - (Vector2) _player.transform.position).normalized;
            var killPointToThis = (_killPoint.Value - (Vector2) transform.position).normalized;
            var dot = Vector2.Dot(killPointToThis, killPointToPlayer);

            if (!_dead && dot < 0 &&
                Vector2.Distance(_player.transform.position, _killPoint.Value) > 0f)
            {
                _dead = true;
                Kill();
            }
        }
    }

    private void Kill()
    {
        OnAnyEnemyDeath?.Invoke();    
        UnitKillPointReached?.Invoke();
        OnDeath?.Invoke();
        StartCoroutine(KillWithDelay());
    }

    private IEnumerator KillWithDelay()
    {
        var currentColorLerpTime = 0f;

        GetComponent<Collider2D>().enabled = false;
        
        while (true)
        {
            currentColorLerpTime += Time.deltaTime;

            if (currentColorLerpTime >= 1f)
            {
                Instantiate(_deathVFX, transform.position, Quaternion.identity);

                _dead = false;
                _killPoint = null;
                gameObject.SetActive(false);
            }
            
            GetComponent<SpriteRenderer>().color =
                Color.Lerp(GetComponent<SpriteRenderer>().color, Color.red, currentColorLerpTime / 1.5f);
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        if (this == null) return;
        
        Gizmos.color = Color.magenta;

        if (_unitAngle != null)
        {
            Gizmos.DrawWireSphere(GetFauxKillPoint(), .5f);
        }

        if (_killPoint.HasValue)
        {
            Gizmos.DrawSphere(_killPoint.Value, .5f);
        }
    }
}

public class UnitKillCleanupHandler : MonoBehaviour
{
    private Color _defaultColor;

    private void Awake()
    {
        _defaultColor = GetComponent<SpriteRenderer>().color;
    }
}