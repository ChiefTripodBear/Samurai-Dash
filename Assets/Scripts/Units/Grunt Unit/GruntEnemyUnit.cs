public class GruntEnemyUnit : EnemyUnit
{
    public override void Register()
    {
        UnitManager<GruntEnemyUnit>.Instance.RegisterUnit(this);
    }

    public override void RemoveFromUnitManager()
    {
        UnitManager<GruntEnemyUnit>.Instance.RemoveUnit(this);
    }
}