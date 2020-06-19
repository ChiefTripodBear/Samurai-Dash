using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public static class Pathfinder
{
    private static Player Player;
    static Pathfinder()
    {
        Player = Object.FindObjectOfType<Player>();
    }

    public static Vector2[] Path(IUnitEnemy unitEnemy, Vector2 targetPosition)
    {
        var startingNode = NodeGrid.Instance.NodeFromWorldPosition(unitEnemy.Transform.position);
        var targetNode = NodeGrid.Instance.NodeFromWorldPosition(targetPosition);

        if(GameUnitManager.IsValidNodeFromUnit(targetNode, unitEnemy) && targetNode.IsWalkable)
        {
            var openSet = new Heap<Node>(NodeGrid.Instance.MaxSize);
            var closedSet = new HashSet<Node>();

            openSet.Add(startingNode);

            while (openSet.Count > 0)
            {
                var currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode) return RetracePath(startingNode, targetNode);

                foreach (var neighbor in NodeGrid.Instance.Neighbors(currentNode, 3))
                {
                    if (!GameUnitManager.IsValidNodeFromUnit(neighbor, unitEnemy) ||
                        !GameUnitManager.IsValidNodeFromPlayer(neighbor, Player) || !neighbor.IsWalkable ||
                        closedSet.Contains(neighbor))
                        continue;

                    var newMovementCostToNeighbor = currentNode.GCost + GetDistance(currentNode, neighbor);

                    if (newMovementCostToNeighbor < neighbor.GCost || !openSet.Contains(neighbor))
                    {
                        neighbor.GCost = newMovementCostToNeighbor;
                        neighbor.HCost = GetDistance(neighbor, targetNode);
                        neighbor.Parent = currentNode;

                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                        else
                            openSet.UpdateItem(neighbor);
                    }
                }
            }
        }

        return null;
    }

    private static Vector2[] RetracePath(Node startingNode, Node targetNode)
    {
        var path = new List<Node>();
        
        var currentNode = targetNode;

        while (currentNode != startingNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }

        path.Add(startingNode);
        var wayPoints = SimplifyPath(path);

        Array.Reverse(wayPoints);
        
        return wayPoints;
    }

    private static Vector2[] SimplifyPath(List<Node> path)
    {
        var wayPoints = new List<Vector2>();

        var directionOld = Vector2.zero;

        for (var i = 1; i < path.Count; i++)
        {
            var directionNew = new Vector2(path[i - 1].GridX - path[i].GridX, path[i - 1].GridY - path[i].GridY);
            if(directionNew != directionOld)
                wayPoints.Add(path[i - 1].WorldPosition);

            directionOld = directionNew;
        }

        return wayPoints.ToArray();
    }

    private static int GetDistance(Node nodeA, Node nodeB)
    {
        var distanceX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
        var distanceY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

        if (distanceX > distanceY)
            return 14 * distanceY + 10 * (distanceX - distanceY);
        return 14 * distanceX + 10 * (distanceY - distanceX);
    }
}