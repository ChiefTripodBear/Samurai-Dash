public class RangedEnemyUnit : EnemyUnit
{
    public override IUnitAttack UnitAttack { get; protected set; }
    public override UnitManager UnitManager { get; protected set; }

    protected override void Awake()
    {
        UnitAttack = GetComponent<RangedAttack>();
        UnitManager = FindObjectOfType<RangedUnitManager>();

        base.Awake();
    }
}