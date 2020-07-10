using UnityEngine;

[RequireComponent(typeof(AngleDefinition))]
[RequireComponent(typeof(UnitKillHandler))]
public abstract class EnemyUnit : PooledMonoBehaviour, IUnitEnemy
{
    public MonoBehaviour MonoBehaviour => this;
    public AngleDefinition AngleDefinition { get; private set; }
    public UnitKillHandler KillHandler { get; private set; }
    public Transform Transform => transform;
    public EnemyUnitMovementController EnemyUnitMovementController { get; private set; }

    public abstract UnitManager UnitManager { get; protected set; }
    public abstract IUnitAttack UnitAttack { get; protected set; }

    public Node CurrentNode { get; private set; }
    private Color _defaultColor;

    protected virtual void Awake()
    {
        _defaultColor = GetComponent<SpriteRenderer>().color;
        AngleDefinition = GetComponent<AngleDefinition>();
        KillHandler = GetComponent<UnitKillHandler>();
        EnemyUnitMovementController = new EnemyUnitMovementController(this);
        KillHandler.OnDeath += RemoveFromUnitManager;
    }
    
    private void OnDestroy()
    {
        KillHandler.OnDeath -= RemoveFromUnitManager;
        RemoveFromUnitManager();
    }

    protected virtual void Update()
    {
        CurrentNode = NodeGrid.Instance.NodeFromWorldPosition(transform.position);
        EnemyUnitMovementController.Tick();
    }
    
    public void Register()
    {
        UnitManager.RegisterUnit(this);
        GetComponent<SpriteRenderer>().color = _defaultColor;
        GetComponent<Collider2D>().enabled = true;
    }

    public void MoveFromSpawn(Vector2 point)
    {
        EnemyUnitMovementController.RequestSpawnPath(point);
    }
    
    private void RemoveFromUnitManager()
    {
        UnitManager.RemoveUnit(this);
    }
}