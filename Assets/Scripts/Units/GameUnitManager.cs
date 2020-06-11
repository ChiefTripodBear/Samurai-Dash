using System.Collections.Generic;

public static class GameUnitManager
{
    private static readonly List<IUnit> _allUnits = new List<IUnit>();
    
    public static void RegisterUnit(IUnit unit)
    {
        _allUnits.Add(unit);
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
}
