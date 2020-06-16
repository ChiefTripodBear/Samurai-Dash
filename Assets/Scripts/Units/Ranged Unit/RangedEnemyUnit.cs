public class RangedEnemyUnit : EnemyUnit
{
    public override EnemyUnitMover EnemyUnitMover { get; protected set; }
    public override UnitManager UnitManager { get; protected set; }

    protected override void Awake()
    {
        EnemyUnitMover = new RangedUnitMover();
        UnitManager = FindObjectOfType<RangedUnitManager>();

        base.Awake();
    }
}