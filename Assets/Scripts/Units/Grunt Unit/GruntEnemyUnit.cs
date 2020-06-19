public class GruntEnemyUnit : EnemyUnit
{
    public override IUnitAttack UnitAttack { get; protected set; }
    public override UnitManager UnitManager { get; protected set; }

    protected override void Awake()
    {
        UnitAttack = GetComponent<GruntAttack>();
        UnitManager = FindObjectOfType<GruntUnitManager>();
        base.Awake();
    }
}