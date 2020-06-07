using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(UnitAngle))]
[RequireComponent(typeof(UnitKillHandler))]
public class RangedUnitProjectile : PooledMonoBehaviour, IKillableWithAngle
{
    public UnitAngle UnitAngle { get; private set; }
    public UnitKillHandler UnitKillHandler { get; private set; }
    public Transform Transform => transform;
    private Rigidbody2D _rigidbody2D;
    
    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        UnitAngle = GetComponent<UnitAngle>();
        UnitKillHandler = GetComponent<UnitKillHandler>();
    }

    private void Start()
    {
        UnitChainEvaluator.Instance.RegisterKillableUnit(this);
    }

    public void Launch(Vector3 shotDirection)
    {
        _rigidbody2D.AddForce(shotDirection, ForceMode2D.Impulse);
    }
}