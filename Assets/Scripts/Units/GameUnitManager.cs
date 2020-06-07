using System.Collections.Generic;
using UnityEngine;

public static class GameUnitManager
{
    private static readonly List<Unit> _allUnits = new List<Unit>();
    
    public static void RegisterUnit(Unit unit)
    {
        _allUnits.Add(unit);
    }

    public static void RemoveUnit(Unit unit)
    {
        _allUnits.Remove(unit);
    }
    
    public static bool IsValidNodeFromUnit(Node nodeQuery, Unit unitQuery)
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
}
