using System.Collections.Generic;
using UnityEngine;

public static class GameUnitManager
{
    private static readonly List<IUnit> _allUnits = new List<IUnit>();
    private static Collider2D[] _fearedEnemyResults = new Collider2D[20];
    private static LayerMask EnemyUnitMask => LayerMask.GetMask("Enemy");
    
    public static void RegisterUnit(IUnit unit)
    {
        _allUnits.Add(unit);
    }

    public static int UnitsOnScreenFromPercent(float percent)
    {
        return Mathf.CeilToInt(_allUnits.Count * percent);
    }

    public static void RemoveUnit(IUnit unit)
    {
        _allUnits.Remove(unit);
    }
    
    public static bool IsValidNodeFromUnit(Node nodeQuery, IUnit unitQuery)
    {
        foreach (var unit in _allUnits)
            if (unit != unitQuery && unit.CurrentNode == nodeQuery)
                return false;

        return true;
    }

    public static bool IsValidNodeFromPlayer(Node nodeQuery, Player player)
    {
        return nodeQuery != player.CurrentNode;
    }

    public static void PerformPostKillFear(Player player)
    {
        var size =
            Physics2D.OverlapCircleNonAlloc(player.transform.position, 8f, _fearedEnemyResults, EnemyUnitMask);

        for (var i = 0; i < size; i++)
        {
            var unit = _fearedEnemyResults[i].GetComponent<IUnitEnemy>();
            
            if(unit?.EnemyUnitMovementController?.UnitEventSpecificMovements == null || !unit.EnemyUnitMovementController.UnitEventSpecificMovements.CanBeFeared) continue;
            
            unit.EnemyUnitMovementController.UnitEventSpecificMovements.PerformFear();
        }
    }

    public static void Clear()
    {
        _allUnits.Clear();
    }
}