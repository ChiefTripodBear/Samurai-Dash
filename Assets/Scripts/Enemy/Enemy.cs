using UnityEngine;

[RequireComponent(typeof(PathfindingUnit))]
[RequireComponent(typeof(EnemyAngle))]
[RequireComponent(typeof(EnemyKillHandler))]
public class Enemy : MonoBehaviour
{
    public PathfindingUnit PathfindingUnit { get; private set; }
    public EnemyAngle EnemyAngle { get; private set; }
    public EnemyKillHandler EnemyKillHandler { get; private set; }

    private void Awake()
    {
        PathfindingUnit = GetComponent<PathfindingUnit>();
        EnemyAngle = GetComponent<EnemyAngle>();
        EnemyKillHandler = GetComponent<EnemyKillHandler>();
    }
}