using System;
using UnityEngine;

[RequireComponent(typeof(AngleDefinition))]
[RequireComponent(typeof(UnitKillHandler))]
public abstract class EnemyUnit : PooledMonoBehaviour, IUnitEnemy
{
    public event Action OnActivated;
    public UnitEventSpecificMovements UnitEventSpecificMovements { get; private set; }
    public IUnitAttack UnitAttack { get; private set; }
    public AngleDefinition AngleDefinition { get; private set; }
    public UnitKillHandler KillHandler { get; private set; }
    public UnitMovementManager UnitMovementManager { get; private set; }

    public Transform Transform => transform;

    private NodeGrid _nodeGrid;
    public Node CurrentNode { get; private set; }

    public abstract EnemyUnitMover EnemyUnitMover { get; protected set; }
    public abstract UnitManager UnitManager { get; protected set; }

    protected virtual void Awake()
    {
        _nodeGrid = FindObjectOfType<NodeGrid>();
        UnitEventSpecificMovements = new UnitEventSpecificMovements(this, FindObjectOfType<Player>());
        UnitAttack = GetComponent<IUnitAttack>();
        AngleDefinition = GetComponent<AngleDefinition>();
        KillHandler = GetComponent<UnitKillHandler>();
        UnitMovementManager = new UnitMovementManager(transform, this);
    }
    
    private void Update()
    {
        CurrentNode = _nodeGrid.NodeFromWorldPosition(transform.position);
    }


    public void Register()
    {
        OnActivated?.Invoke();
        UnitManager.RegisterUnit(this);
    }
    
    public void RemoveFromUnitManager()
    {
        UnitManager.RemoveUnit(this);
    }
}