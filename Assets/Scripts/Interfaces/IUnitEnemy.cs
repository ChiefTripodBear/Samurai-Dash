using UnityEngine;

public interface IUnitEnemy : IUnit
{
    MonoBehaviour MonoBehaviour { get; }
    IUnitAttack UnitAttack { get; }
    UnitEventSpecificMovements UnitEventSpecificMovements { get; }
    EnemyUnitMover EnemyUnitMover { get; }
    UnitManager UnitManager { get; }
    UnitMovementManager UnitMovementManager { get; }
}