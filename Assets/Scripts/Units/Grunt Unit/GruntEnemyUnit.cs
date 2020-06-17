public class GruntEnemyUnit : EnemyUnit
{
    public override IUnitAttack UnitAttack { get; protected set; }
    public override EnemyUnitMover EnemyUnitMover { get; protected set; }
    public override UnitManager UnitManager { get; protected set; }

    protected override void Awake()
    {
        UnitAttack = GetComponent<GruntAttack>();
        EnemyUnitMover = new GruntUnitMover(UnitAttack);
        UnitManager = FindObjectOfType<GruntUnitManager>();
        base.Awake();
    }
}