using UnityEngine;

public interface IKillableWithAngle
{
    UnitAngle UnitAngle { get; }
    UnitKillHandler UnitKillHandler { get; }
    Transform Transform { get; }
}