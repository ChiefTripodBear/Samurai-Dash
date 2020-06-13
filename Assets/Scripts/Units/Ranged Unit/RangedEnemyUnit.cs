public class RangedEnemyUnit : EnemyUnit
{
    public override void Register()
    {
        UnitManager<RangedEnemyUnit>.Instance.RegisterUnit(this);
    }

    public override void RemoveFromUnitManager()
    {
        UnitManager<RangedEnemyUnit>.Instance.RegisterUnit(this);
    }
}