using System.Collections.Generic;

public class IntersectionDetector
{
    public Queue<IUnit> GetIntersectionsFromUnit(IUnit fromUnit)
    {
        return fromUnit != null ? UnitChainEvaluator.GetIntersectionsRelativeTo(fromUnit) : null;
    }
}