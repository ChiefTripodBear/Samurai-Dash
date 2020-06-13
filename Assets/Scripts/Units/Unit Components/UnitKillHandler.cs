using System;
using System.Collections;
using UnityEngine;

public class UnitKillHandler : MonoBehaviour
{
    public event Action OnDeath;

    [SerializeField] private float _killPointDistance = 1.5f;
    [SerializeField] private GameObject _deathVFX;

    private bool _alreadyRegisteredToKillQueue;
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

    private void OnEnable()
    {
        GetComponent<SpriteRenderer>().color = _defaultColor;
        GetComponent<Collider2D>().enabled = true;
    }

    private void OnDisable()
    {
        OnDeath?.Invoke();
    }

    public void SetKillPoint()
    {
        _killPoint = ((Vector2)transform.position - _unitAngle.GetPointClosestTo(_player.transform.position)).normalized * _killPointDistance + (Vector2)transform.position;
    }

    public Vector2 GetFauxKillPoint()
    {
        return ((Vector2)transform.position - _unitAngle.GetPointClosestTo(_player.transform.position)).normalized * _killPointDistance + (Vector2)transform.position;
    }

    private void Update()
    {
        if (_player != null && _killPoint != null)
        {
            var killPointToPlayer = (_killPoint.Value - (Vector2) _player.transform.position).normalized;
            var killPointToThis = (_killPoint.Value - (Vector2) transform.position).normalized;
            var dot = Vector2.Dot(killPointToThis, killPointToPlayer);

            if (!_alreadyRegisteredToKillQueue && dot < 0 &&
                Vector2.Distance(_player.transform.position, _killPoint.Value) > 0.1f)
            {
                _alreadyRegisteredToKillQueue = true;
                Kill();
            }
        }
    }

    private void Kill()
    {
        UnitChainEvaluator.Instance.RemoveUnit(GetComponent<IUnit>());
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

                _alreadyRegisteredToKillQueue = false;
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
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(GetFauxKillPoint(), .5f);
    }
}