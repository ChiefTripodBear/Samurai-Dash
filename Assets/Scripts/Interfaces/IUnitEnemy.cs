public interface IUnitEnemy : IUnit
{
    IUnitAttack UnitAttack { get; }
    UnitEventSpecificMovements UnitEventSpecificMovements { get; }
    EnemyUnitMover EnemyUnitMover { get; }
    UnitManager UnitManager { get; }
    UnitMovementManager UnitMovementManager { get; }
}