using UnityEngine;

public class DummyUnit : MonoBehaviour, IUnit
{
    public AngleDefinition AngleDefinition { get; private set; }
    public UnitKillHandler KillHandler { get; private set; }
    public Transform Transform => transform;
    public IUnitPathFinder UnitPathFinder { get; } = null;
    public IUnitAttack UnitAttack { get; } = null;
    public Node CurrentNode { get; private set; }

    private NodeGrid _nodeGrid;

    private void Awake()
    {
        _nodeGrid = FindObjectOfType<NodeGrid>();
        AngleDefinition = GetComponent<AngleDefinition>();
        KillHandler = GetComponent<UnitKillHandler>();
    }

    private void Start()
    {
        UnitChainEvaluator.Instance.AddUnit(this);
        GameUnitManager.RegisterUnit(this);
    }

    private void Update()
    {
        CurrentNode = _nodeGrid.NodeFromWorldPosition(transform.position);
    }
}