using UnityEngine;

[RequireComponent(typeof(UnitAngle))]
[RequireComponent(typeof(UnitKillHandler))]
public abstract class Unit : PooledMonoBehaviour, IKillableWithAngle
{
    public IUnitPathFinder UnitPathFinder { get; private set; }
    public IUnitAttack UnitAttack { get; private set; }
    public UnitAngle UnitAngle { get; private set; }
    public UnitKillHandler UnitKillHandler { get; private set; }
    public Transform Transform => transform;

    private NodeGrid _nodeGrid;
    public Node CurrentNode { get; private set; }

    private void Awake()
    {
        _nodeGrid = FindObjectOfType<NodeGrid>();
        UnitAttack = GetComponent<IUnitAttack>();
        UnitPathFinder = GetComponent<IUnitPathFinder>();
        UnitAngle = GetComponent<UnitAngle>();
        UnitKillHandler = GetComponent<UnitKillHandler>();
    }
    
    private void Update()
    {
        CurrentNode = _nodeGrid.NodeFromWorldPosition(transform.position);
        
        UnitPathFinder.Tick();
    }

    public abstract void Register();
    public abstract void RemoveFromUnitManager();
}