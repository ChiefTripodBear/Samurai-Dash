using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private float _moveAmountPerSwipe = 5f;
    [SerializeField] private float _moveSpeed;
    public float DefaultMovementSpeed => _moveSpeed;
    public float MoveAmountPerSwipe => _moveAmountPerSwipe;

    private NodeGrid _grid;
    public Node CurrentNode { get; private set; }
    private PlayerMovementManager _playerMovementManager;
    private bool _reloaded;

    private void Awake()
    {
        _playerMovementManager = new PlayerMovementManager(this);
        _grid = FindObjectOfType<NodeGrid>();
        Time.timeScale = 1f;
    }
    
    private void Update()
    {
        GetComponent<Collider2D>().enabled = MovementPackage.MovementCount <= 0;
        _playerMovementManager.Tick();
        
        CurrentNode = _grid.NodeFromWorldPosition(transform.position);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        var enemy = other.GetComponent<IUnit>();

        if (enemy == null) return;

        if (_reloaded) return;

        _reloaded = true;
        Time.timeScale = 1f;
        Pool.ClearPools();
        SceneManager.LoadScene(0);
    }
}