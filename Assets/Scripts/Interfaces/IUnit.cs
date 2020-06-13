using UnityEngine;

public interface IUnit
{
    AngleDefinition AngleDefinition { get; }
    UnitKillHandler KillHandler { get; }
    Transform Transform { get; }
    Node CurrentNode { get; }
}