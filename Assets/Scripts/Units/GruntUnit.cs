public class GruntUnit : Unit
{
    public override void Register()
    {
        UnitManager<GruntUnit>.Instance.RegisterUnit(this);
    }

    public override void RemoveFromUnitManager()
    {
        UnitManager<GruntUnit>.Instance.RemoveUnit(this);
    }
}