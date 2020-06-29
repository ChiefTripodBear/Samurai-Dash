using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class RingPathFinder
{
    public static Vector2[] Path(IUnitEnemy unitEnemy, RingNode startingNode, RingNode targetRingNode)
    {
        if (!targetRingNode.IsWalkable() || !startingNode.IsWalkable()) return null;

        Vector2[] pathToStartingRingNode = null;
        if (unitEnemy != null)
        {
            pathToStartingRingNode = Pathfinder.Path(unitEnemy, startingNode.RingPosition.transform.position);
        }
        
        var openSet = new Heap<RingNode>(RingManager.Instance.AllPositions.Count);
        var closedSet = new HashSet<RingNode>();

        openSet.Add(startingNode);

        while (openSet.Count > 0)
        {
            var currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if (currentNode == targetRingNode) return RetracePath(startingNode, targetRingNode, pathToStartingRingNode);

            foreach (var neighbor in RingManager.Instance.GetNeighborsFor(currentNode))
            {
                if (!neighbor.IsWalkable() || closedSet.Contains(neighbor)) continue;

                var newMovementCostToNeighbor = currentNode.GCost + GetDistance(currentNode, neighbor);

                if (newMovementCostToNeighbor < neighbor.GCost || !openSet.Contains(neighbor))
                {
                    neighbor.GCost = newMovementCostToNeighbor;
                    neighbor.HCost = GetDistance(neighbor, targetRingNode);
                    neighbor.ParentRing = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                    else
                        openSet.UpdateItem(neighbor);
                }
            }
        }

        return null;
    }

    private static int GetDistance(RingNode nodeA, RingNode nodeB)
    {
        return (int)Vector2.Distance(nodeA.RingPosition.transform.position, nodeB.RingPosition.transform.position);
    }

    private static Vector2[] RetracePath(RingNode startingNode, RingNode targetRingNode,
        Vector2[] pathToStartingRingNode)
    {
        var ringNodes = new List<RingPosition>();

        var currentRingNode = targetRingNode;

        while (currentRingNode != startingNode)
        {
            ringNodes.Add(currentRingNode.RingPosition);
            currentRingNode = currentRingNode.ParentRing;
        }
        
        ringNodes.Add(startingNode.RingPosition);
        var positions = ringNodes.Select(t => (Vector2)t.transform.position).ToList();
        positions.Reverse();

        if (pathToStartingRingNode != null && pathToStartingRingNode.Length > 0)
        {
            for (var i = pathToStartingRingNode.Length - 1; i > 0; i--)
            {
                positions.Insert(0, pathToStartingRingNode[i]);
            }
        }
        
        return positions.ToArray();
    }
}