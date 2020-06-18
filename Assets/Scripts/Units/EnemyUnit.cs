using UnityEngine;

[RequireComponent(typeof(AngleDefinition))]
[RequireComponent(typeof(UnitKillHandler))]
public abstract class EnemyUnit : PooledMonoBehaviour, IUnitEnemy
{
    public UnitEventSpecificMovements UnitEventSpecificMovements { get; private set; }
    public MonoBehaviour MonoBehaviour => this;
    public abstract IUnitAttack UnitAttack { get; protected set; }
    public AngleDefinition AngleDefinition { get; private set; }
    public UnitKillHandler KillHandler { get; private set; }
    public UnitMovementManager UnitMovementManager { get; private set; }
    public Transform Transform => transform;

    private NodeGrid _nodeGrid;
    private Color _defaultColor;
    public Node CurrentNode { get; private set; }

    public abstract EnemyUnitMover EnemyUnitMover { get; protected set; }
    public abstract UnitManager UnitManager { get; protected set; }

    protected virtual void Awake()
    {
        _defaultColor = GetComponent<SpriteRenderer>().color;
        _nodeGrid = FindObjectOfType<NodeGrid>();
        UnitEventSpecificMovements = new UnitEventSpecificMovements(this, FindObjectOfType<Player>());
        AngleDefinition = GetComponent<AngleDefinition>();
        KillHandler = GetComponent<UnitKillHandler>();
        UnitMovementManager = new UnitMovementManager(transform, this);

        KillHandler.OnDeath += RemoveFromUnitManager;
    }

    private void OnDestroy()
    {
        KillHandler.OnDeath -= RemoveFromUnitManager;
    }

    private void Update()
    {
        CurrentNode = _nodeGrid.NodeFromWorldPosition(transform.position);
    }
    
    public void Register()
    {
        UnitManager.RegisterUnit(this);
        GetComponent<SpriteRenderer>().color = _defaultColor;
        GetComponent<Collider2D>().enabled = true;
    }

    public void MoveFromSpawn(Vector2 point)
    {
        UnitMovementManager.MoveToPoint(point, true, 0f, 0f);
    }
    
    private void RemoveFromUnitManager()
    {
        UnitManager.RemoveUnit(this);
    }

    private void OnDrawGizmos()
    {
        EnemyUnitMover?.DrawGizmos();
    }
}