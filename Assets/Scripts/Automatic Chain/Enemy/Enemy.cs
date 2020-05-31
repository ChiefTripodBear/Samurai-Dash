using System;
using System.Collections;
using UnityEngine;

public class Enemy : PooledMonoBehaviour
{
    [SerializeField] private GameObject _deathVFX;
    public EnemyChainHandler EnemyChainHandler { get; private set; }
    public EnemyAngleDefinition EnemyAngleDefinition { get; private set; }
    public SpriteRenderer SpriteRenderer { get; private set; }

    public event Action OnDeath;

    private EnemyMover _enemyMover;
    
    private void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        EnemyChainHandler = GetComponent<EnemyChainHandler>();
        EnemyAngleDefinition = GetComponent<EnemyAngleDefinition>();
        _enemyMover = GetComponent<EnemyMover>();
    }
    
    public void Kill()
    {
        StartCoroutine(KillWithDelay());
    }

    private IEnumerator KillWithDelay()
    {
        OnDeath?.Invoke();
        _enemyMover.CanMove = false;

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