using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyChainEvaluator : MonoBehaviour
{
    private ChainFinder _chainFinder;

    private List<Enemy> _enemies = new List<Enemy>();

    private void Awake()
    {
        _chainFinder = GetComponent<ChainFinder>();
        _enemies = FindObjectsOfType<Enemy>().ToList();
        
        _enemies.ForEach(t => t.OnDeath += () =>
        {
            if (_bestEnemy != null && t == _bestEnemy)
            {
                _bestChainCount = 0;
                _bestEnemy = null;
            }
            _enemies.Remove(t);
        });
    }

    private int _bestChainCount;
    private Enemy _bestEnemy;

    private void Update()
    {
        var orderedChains = _enemies.Where(t => t.EnemyChainHandler.Chain != null).OrderByDescending(t => t.EnemyChainHandler.Chain.Count);

        if (orderedChains.Any())
        {
            var best = orderedChains.ElementAt(0);

            if (_bestEnemy != null)
            {
                _bestEnemy.SpriteRenderer.color = Color.white;
            }

            _bestEnemy = best;

            _bestEnemy.SpriteRenderer.color = Color.yellow;
        }
    }
}