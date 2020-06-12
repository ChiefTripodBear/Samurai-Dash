using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class Boundary : MonoBehaviour
{
    private Vector2 _size;
    private float _nodeRadius = 0.5f;
    private float _nodeDiameter;

    private BoundaryNode[] _boundaryGrid;
    private Vector2 _bottomRight;
    private Vector2 _topLeft;
    
    private List<Vector2> _positions = new List<Vector2>();
    private Vector2 _bottomLeft;
    private Vector2 _topRight;
    private List<BoundaryNode> _bestPath;
    private List<Vector2> _testPath;

    [SerializeField] private Transform _start;
    [SerializeField] private Transform _destination;
    private void Awake()
    {
        _nodeDiameter = _nodeRadius * 2;
        _size = transform.localScale;
        BuildGrid();
    }
    
    private void BuildGrid()
    {
        var width = _size.x;
        var height = _size.y;
        var numberOfNodes = (int) (width * 2 + height * 2 + 4);
        _boundaryGrid = new BoundaryNode[numberOfNodes];

        _bottomRight = (Vector2) transform.position + Vector2.right * width / 2 - Vector2.up * height / 2 -
            Vector2.right * _nodeRadius + Vector2.up * _nodeRadius;
        _topLeft = (Vector2) transform.position - Vector2.right * width / 2 + Vector2.up * height / 2 +
            Vector2.right * _nodeRadius - Vector2.up * _nodeRadius;
        _bottomLeft = (Vector2) transform.position - Vector2.right * width / 2 - Vector2.up * height / 2 +
                      Vector2.right * _nodeRadius + Vector2.up * _nodeRadius;
        _topRight = (Vector2) transform.position + Vector2.right * width / 2 + Vector2.up * height / 2 -
                    Vector2.right * _nodeRadius - Vector2.up * _nodeRadius;

        var nodeCountTracker = 0;
        for (var i = 0; i < 4; i++)
        {
            var isWidth = i == 0 || i % 2 == 0;

            var nodeCount = isWidth ? width : height;

            var startingPoint = Vector2.zero;
            var direction = Vector2.zero;

            switch (i)
            {
                case 0:
                    startingPoint = _bottomRight - Vector2.up * _nodeDiameter;
                    direction = Vector2.left;
                    break;
                case 1:
                    startingPoint = _bottomLeft - Vector2.right * _nodeDiameter;
                    direction = Vector2.up;
                    break;
                case 2:
                    startingPoint = _topLeft + Vector2.up * _nodeDiameter;
                    direction = Vector2.right;
                    break;
                case 3:
                    startingPoint = _topRight + Vector2.right * _nodeDiameter;
                    direction = Vector2.down;
                    break;
            }

            for (var j = 0; j <= nodeCount; j++)
            {
                var nodePosition = startingPoint + direction * _nodeDiameter * j;

                _positions.Add(nodePosition);

                _boundaryGrid[nodeCountTracker] = new BoundaryNode(nodePosition, nodeCountTracker);

                nodeCountTracker++;
            }
        }

        foreach (var node in _boundaryGrid)
        {
            var previousNeighbor = node.NodeIndex == 0 ? _boundaryGrid[_boundaryGrid.Length - 1] : _boundaryGrid[node.NodeIndex - 1];
            var nextNeighbor = node.NodeIndex == _boundaryGrid.Length - 1 ? _boundaryGrid[0] : _boundaryGrid[node.NodeIndex + 1];
            node.SetNeighbors(previousNeighbor, nextNeighbor);
        }
    }

    private void Update()
    {
        if (Mouse.current.middleButton.wasReleasedThisFrame)
        {
            DisplayBestPath();
        }
    }

    public List<Vector2> BoundaryPath(Vector2 targetLocation, Vector2 pointClosestToBoundary)
    {
        var closestBoundaryNodeToStart = _boundaryGrid
            .OrderBy(t => Vector2.Distance(pointClosestToBoundary, t.BoundaryNodePosition)).FirstOrDefault();
        
        var closestBoundaryNodeToTargetDestination = _boundaryGrid
            .OrderBy(t => Vector2.Distance(targetLocation, t.BoundaryNodePosition)).FirstOrDefault();

        _bestPath = BestPath(closestBoundaryNodeToStart, closestBoundaryNodeToTargetDestination);

        return _bestPath.Select(t => t.BoundaryNodePosition).ToList();
    }

    private void DisplayBestPath()
    {
        _testPath = BoundaryPath(_start.position, _destination.position);
    }
    
    private List<BoundaryNode> BestPath(BoundaryNode closestBoundaryNodeToStart, BoundaryNode closestBoundaryNodeToTargetDestination)
    {
        var pathA = GeneratePath(closestBoundaryNodeToTargetDestination, closestBoundaryNodeToStart, true);
        var pathB = GeneratePath(closestBoundaryNodeToTargetDestination, closestBoundaryNodeToStart, false);
        return pathA.Count < pathB.Count ? pathA : pathB;
    }

    private List<BoundaryNode> GeneratePath(BoundaryNode closestBoundaryNodeToTargetDestination, BoundaryNode closestBoundaryNodeToStart, bool nextNeighbor)
    {
        var path = new List<BoundaryNode> {closestBoundaryNodeToStart};
        
        var currentNode = closestBoundaryNodeToStart;

        while (currentNode != closestBoundaryNodeToTargetDestination)
        {
            currentNode = nextNeighbor ? currentNode.NextNeighbor : currentNode.PreviousNeighbor;
            path.Add(currentNode);
        }

        return path;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_bottomRight, Vector2.one);
        Gizmos.DrawWireCube(_topLeft, Vector2.one);
        Gizmos.DrawWireCube(_topRight, Vector2.one);
        Gizmos.DrawWireCube(_bottomLeft, Vector2.one);

        foreach (var nodePosition in _positions)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(nodePosition, Vector2.one);
        }

        if (_testPath != null)
            foreach (var position in _testPath)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawCube(position, Vector2.one);
            }
    }
}