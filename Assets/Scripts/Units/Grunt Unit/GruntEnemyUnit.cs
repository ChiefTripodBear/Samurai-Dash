public class GruntEnemyUnit : EnemyUnit
{
    public override EnemyUnitMover EnemyUnitMover { get; protected set; }
    public override UnitManager UnitManager { get; protected set; }

    protected override void Awake()
    {
        EnemyUnitMover = new GruntUnitMover();
        UnitManager = FindObjectOfType<GruntUnitManager>();
        base.Awake();
    }
}