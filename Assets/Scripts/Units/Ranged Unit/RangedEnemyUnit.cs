public class RangedEnemyUnit : EnemyUnit
{
    public override IUnitAttack UnitAttack { get; protected set; }
    public override EnemyUnitMover EnemyUnitMover { get; protected set; }
    public override UnitManager UnitManager { get; protected set; }

    protected override void Awake()
    {
        UnitAttack = GetComponent<RangedAttack>();
        EnemyUnitMover = new RangedUnitMover(UnitAttack);
        UnitManager = FindObjectOfType<RangedUnitManager>();

        base.Awake();
    }
}