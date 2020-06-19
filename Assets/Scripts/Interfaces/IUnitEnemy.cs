using UnityEngine;

public interface IUnitEnemy : IUnit
{
    EnemyUnitMovementController EnemyUnitMovementController { get; }
    MonoBehaviour MonoBehaviour { get; }
    IUnitAttack UnitAttack { get; }
    UnitManager UnitManager { get; }
}