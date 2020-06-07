public class RangedUnit : Unit
{
    public override void Register()
    {
        UnitManager<RangedUnit>.Instance.RegisterUnit(this);
    }

    public override void RemoveFromUnitManager()
    {
        UnitManager<RangedUnit>.Instance.RegisterUnit(this);
    }
}