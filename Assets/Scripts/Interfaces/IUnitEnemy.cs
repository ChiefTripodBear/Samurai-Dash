using System;

public interface IUnitEnemy : IUnit
{
    event Action OnActivated;
    IUnitAttack UnitAttack { get; }
    UnitEventSpecificMovements UnitEventSpecificMovements { get; }
    EnemyUnitMover EnemyUnitMover { get; }
    UnitManager UnitManager { get; }
    UnitMovementManager UnitMovementManager { get; }
}