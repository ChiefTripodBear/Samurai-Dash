using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private bool _reloaded = false;
    public Node CurrentNode { get; private set; }

    private NodeGrid _grid;
    
    private void Awake()
    {
        _grid = FindObjectOfType<NodeGrid>();
        Time.timeScale = 1f;
    }

    private void Update()
    {
        CurrentNode = _grid.NodeFromWorldPosition(transform.position);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var enemy = other.GetComponent<IKillableWithAngle>();

        if (enemy == null) return;

        if (_reloaded) return;

        _reloaded = true;
        Time.timeScale = 1f;
        Pool.ClearPools();
        SceneManager.LoadScene(0);
    }
}