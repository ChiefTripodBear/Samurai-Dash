using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NodeGrid : MonoBehaviour
{
    [SerializeField] private bool _drawGizmos;
    [SerializeField] private Vector2 _gridSize;
    [SerializeField] private float _nodeRadius = 0.5f;

    public Vector2 GridSize => _gridSize;

    private int _nodeCountX, _nodeCountY;
    private float _nodeDiameter;

    private Node[,] _grid;
    private LayerMask _unwalkableLayer;
    public int MaxSize => _nodeCountX * _nodeCountY;

    private static NodeGrid _instance;
    public static NodeGrid Instance => _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        _nodeDiameter = _nodeRadius * 2;
        _nodeCountX = Mathf.RoundToInt(_gridSize.x / _nodeDiameter);
        _nodeCountY = Mathf.RoundToInt(_gridSize.y / _nodeDiameter);

        _unwalkableLayer = LayerMask.GetMask("Unwalkable");
        BuildGrid();
    }

    private void BuildGrid()
    {
        _grid = new Node[_nodeCountX, _nodeCountY];
        var worldBottomLeft = (Vector2) transform.position - Vector2.right * _gridSize.x / 2 + Vector2.down * _gridSize.y / 2;

        for (var x = 0; x < _nodeCountX; x++)
        for (var y = 0; y < _nodeCountY; y++)
        {
            var position = worldBottomLeft + Vector2.right * (x * _nodeDiameter + _nodeRadius) +
                           Vector2.up * (y * _nodeDiameter + _nodeRadius);
            var walkable = !Physics2D.OverlapCircle(position, _nodeDiameter, _unwalkableLayer);

            _grid[x, y] = new Node(position, x, y, walkable);
        }
    }

    public Node NodeFromWorldPosition(Vector2 worldPosition)
    {
        var percentX = Mathf.Clamp01((worldPosition.x + _gridSize.x / 2) / _gridSize.x);
        var percentY = Mathf.Clamp01((worldPosition.y + _gridSize.y / 2) / _gridSize.y);

        var x = Mathf.RoundToInt((_nodeCountX - 1) * percentX);
        var y = Mathf.RoundToInt((_nodeCountY - 1) * percentY);
    
        return _grid[x, y];
    }

    public List<Node> Neighbors(Node node)
    {
        var neighbors = new List<Node>();

        for (var x = -1; x <= 1; x++)
        for (var y = -1; y <= 1; y++)
        {
            if(x == 0 && y == 0) continue;

            var checkX = node.GridX + x;
            var checkY = node.GridY + y;

            if (checkX >= 0 && checkX < _nodeCountX && checkY >= 0 && checkY < _nodeCountY)
            {
                neighbors.Add(_grid[checkX, checkY]);
            }
        }

        return neighbors;
    }

    public bool WorldPositionOnUnwalkableLayer(Vector2 worldPosition)
    {
        var worldPositionNode = NodeFromWorldPosition(worldPosition);

        foreach (var node in _grid)
        {
            if (!node.IsWalkable)
            {
                if (node == worldPositionNode)
                    return true;
            }
        }
        
        return false;
    }
    
    public bool WorldPositionIsOnNodeInSafetyGrid(Vector2 safetyCenter, Vector2 positionToCheck)
    {
        var neighborSafetyNodes = Neighbors(NodeFromWorldPosition(safetyCenter));

        var nodeToCheck = NodeFromWorldPosition(positionToCheck);

        return neighborSafetyNodes.Any(t => t == nodeToCheck);
    }

    private void OnDrawGizmos()
    {
        if (!_drawGizmos) return;
        Gizmos.DrawWireCube(transform.position, _gridSize);

        if (_grid != null)
        {
            foreach (var node in _grid)
            {
                Gizmos.color = node.IsWalkable ? Color.white : Color.red;
                Gizmos.DrawWireCube(node.WorldPosition, Vector2.one * (_nodeDiameter - 0.1f));
            }
        }
    }
}