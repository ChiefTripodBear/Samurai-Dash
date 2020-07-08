using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AngleDefinition))]
[RequireComponent(typeof(UnitKillHandler))]
public class RangedUnitProjectile : PooledMonoBehaviour, IUnit
{
    public AngleDefinition AngleDefinition { get; private set; }
    public UnitKillHandler KillHandler { get; private set; }
    public Node CurrentNode { get; private set; }
    public Transform Transform => transform;
    private Rigidbody2D _rigidbody2D;
    private NodeGrid _gridNode;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        AngleDefinition = GetComponent<AngleDefinition>();
        KillHandler = GetComponent<UnitKillHandler>();
        _gridNode = FindObjectOfType<NodeGrid>();
    }

    private void Start()
    {
        UnitChainEvaluator.Instance.AddUnit(this);
    }

    private void Update()
    {
        CurrentNode = _gridNode.NodeFromWorldPosition(transform.position);
    }

    public void Launch(Vector3 shotDirection)
    {
        _rigidbody2D.AddForce(shotDirection, ForceMode2D.Impulse);
    }
}