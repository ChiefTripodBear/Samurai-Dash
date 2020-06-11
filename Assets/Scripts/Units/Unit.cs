using UnityEngine;

[RequireComponent(typeof(AngleDefinition))]
[RequireComponent(typeof(UnitKillHandler))]
public abstract class Unit : PooledMonoBehaviour, IUnit
{
    public IUnitPathFinder UnitPathFinder { get; private set; }
    public IUnitAttack UnitAttack { get; private set; }
    public AngleDefinition AngleDefinition { get; private set; }
    public UnitKillHandler KillHandler { get; private set; }
    public Transform Transform => transform;

    private NodeGrid _nodeGrid;
    public Node CurrentNode { get; private set; }

    private void Awake()
    {
        _nodeGrid = FindObjectOfType<NodeGrid>();
        UnitAttack = GetComponent<IUnitAttack>();
        UnitPathFinder = GetComponent<IUnitPathFinder>();
        AngleDefinition = GetComponent<AngleDefinition>();
        KillHandler = GetComponent<UnitKillHandler>();
    }
    
    private void Update()
    {
        CurrentNode = _nodeGrid.NodeFromWorldPosition(transform.position);
        
        UnitPathFinder.Tick();
    }

    public abstract void Register();
    public abstract void RemoveFromUnitManager();
}