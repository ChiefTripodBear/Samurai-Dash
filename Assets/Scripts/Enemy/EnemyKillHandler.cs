using System;
using System.Collections;
using UnityEngine;

public class EnemyKillHandler : MonoBehaviour, IKillable
{
    public static event Action<EnemyKillHandler> OnKillPointReached;
    public event Action OnDeath;

    [SerializeField] private float _killPointDistance = 1.5f;
    [SerializeField] private GameObject _deathVFX;

    private bool _alreadyRegisteredToKillQueue;
    private Vector2? _killPoint;
    public Vector2? KillPoint => _killPoint;

    private EnemyAngle _enemyAngle;
    private Player _player;

    private void Awake()
    {
        _player = FindObjectOfType<Player>();
        _enemyAngle = GetComponent<EnemyAngle>();
    }
    
    private void OnDestroy()
    {
        OnDeath?.Invoke();
    }

    public void SetKillPoint()
    {
        _killPoint = ((Vector2)transform.position - _enemyAngle.GetPointClosestTo(_player.transform.position)).normalized * _killPointDistance + (Vector2)transform.position;
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
                OnKillPointReached?.Invoke(this);
            }
        }
    }

    public void Kill()
    {
        StartCoroutine(KillWithDelay());
    }

    private IEnumerator KillWithDelay()
    {
        OnDeath?.Invoke();

        var currentColorLerpTime = 0f;

        GetComponent<CircleCollider2D>().enabled = false;
        
        while (true)
        {
            currentColorLerpTime += Time.deltaTime;

            if (currentColorLerpTime >= 1f)
            {
                Instantiate(_deathVFX, transform.position, Quaternion.identity);

                gameObject.SetActive(false);
            }
            
            GetComponent<SpriteRenderer>().color =
                Color.Lerp(GetComponent<SpriteRenderer>().color, Color.red, currentColorLerpTime / 1.5f);
            yield return null;
        }
    }

}