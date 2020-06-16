using UnityEngine;

public interface IWaypoint
{
    Transform Transform { get; }
    bool IsClaimed { get; }
    void Claim(EnemyUnitMover enemyUnitMover);
}