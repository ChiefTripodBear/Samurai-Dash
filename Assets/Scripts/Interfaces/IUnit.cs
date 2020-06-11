using UnityEngine;

public interface IUnit
{
    AngleDefinition AngleDefinition { get; }
    UnitKillHandler KillHandler { get; }
    Transform Transform { get; }
    IUnitPathFinder UnitPathFinder { get; }
    IUnitAttack UnitAttack { get; }
    Node CurrentNode { get; }
}