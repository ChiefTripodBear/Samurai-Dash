﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    private NodeGrid _nodeGrid;
    private UnitManager _unitManager;
    private PathfindingUnit _pathfindingUnit;

    private void Awake()
    {
        _nodeGrid = FindObjectOfType<NodeGrid>();
        _unitManager = FindObjectOfType<UnitManager>();
        _pathfindingUnit = GetComponent<PathfindingUnit>();
    }
    
    public Vector2[] Path(Vector2 targetPosition, ref bool completedRequest)
    {
        completedRequest = false;
        var startingNode = _nodeGrid.NodeFromWorldPosition(transform.position);
        var targetNode = _nodeGrid.NodeFromWorldPosition(targetPosition);

        if(_unitManager.IsValidNode(targetNode, _pathfindingUnit) && startingNode.IsWalkable && targetNode.IsWalkable)
        {
            var openSet = new Heap<Node>(_nodeGrid.MaxSize);
            var closedSet = new HashSet<Node>();

            openSet.Add(startingNode);

            while (openSet.Count > 0)
            {
                var currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    completedRequest = true;
                    return RetracePath(startingNode, targetNode);
                }

                foreach (var neighbor in _nodeGrid.Neighbors(currentNode))
                {
                    if (!_unitManager.IsValidNode(neighbor, _pathfindingUnit) || !neighbor.IsWalkable || closedSet.Contains(neighbor)) continue;
                    
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

        completedRequest = true;
        return null;
    }

    private Vector2[] RetracePath(Node startingNode, Node targetNode)
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

    private Vector2[] SimplifyPath(List<Node> path)
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

    private int GetDistance(Node nodeA, Node nodeB)
    {
        var distanceX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
        var distanceY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

        if (distanceX > distanceY)
            return 14 * distanceY + 10 * (distanceX - distanceY);
        return 14 * distanceX + 10 * (distanceY - distanceX);
    }
}