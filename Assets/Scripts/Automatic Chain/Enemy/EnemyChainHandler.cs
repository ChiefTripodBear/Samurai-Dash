using System.Collections.Generic;
using UnityEngine;

public class EnemyChainHandler : MonoBehaviour
{
    private Vector2? _intersectionPoint;
    private Queue<Enemy> _chain = new Queue<Enemy>();

    public Queue<Enemy> Chain => _chain;
    public Vector2? IntersectionPoint => _intersectionPoint;

    public int ChainCount { get; set; }

    private AutomaticChainFinder _automaticChainFinder;

    private Enemy _enemy;
    private Player _player;

    private void Awake()
    {
        _player = FindObjectOfType<Player>();
        _enemy = GetComponent<Enemy>();
        _automaticChainFinder = FindObjectOfType<AutomaticChainFinder>();
    }

    public void SetIntersectionPoint(Vector2? point)
    {
        _intersectionPoint = point;
    }
    
    private void Update()
    {
        if (_player == null) return;
        
        if (Vector2.Distance(transform.position, _player.transform.position) < 3f)
        {
            _chain = _automaticChainFinder.BuildChain(_enemy);
        }
    }
}