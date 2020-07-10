using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private bool _noDeath;
    [SerializeField] private float _moveAmountPerSwipe = 5f;
    [SerializeField] private float _moveSpeed;
    public float DefaultMovementSpeed => _moveSpeed;
    public float MoveAmountPerSwipe => _moveAmountPerSwipe;

    private NodeGrid _grid;
    public Node CurrentNode { get; private set; }
    private PlayerMovementController _playerMovementController;
    private bool _reloaded;
    private PlayerMovementManager _playerMovementManager;
    public Collider2D PlayerCollider2D { get; private set; }

    private void Awake()
    {
        PlayerCollider2D = GetComponent<Collider2D>();
        _grid = FindObjectOfType<NodeGrid>();
        Time.timeScale = 1f;
    }

    private void Start()
    {
        _playerMovementController = new PlayerMovementController(this);
    }

    private void Update()
    {
        _playerMovementController.Tick();
        
        CurrentNode = _grid.NodeFromWorldPosition(transform.position);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_noDeath) return;
        
        var enemy = other.GetComponent<IUnit>();
        
        if (enemy == null) return;
        
        if (_reloaded) return;
        
        _reloaded = true;
        Reload();
    }

    public static void Reload()
    {
        Time.timeScale = 1f;
        Pool.ClearPools();
        GameUnitManager.Clear();
        UnitChainEvaluator.Instance.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene("Game Menu UI Scene", LoadSceneMode.Additive);
    }

    private void OnDrawGizmos()
    {
        _playerMovementController?.DrawGizmos();
    }
}