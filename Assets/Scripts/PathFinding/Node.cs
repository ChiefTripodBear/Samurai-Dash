using UnityEngine;

public class Node : IHeap<Node>
{
    public int FCost => HCost + GCost;
    public int HCost { get; set; }
    public int GCost { get; set; }

    public Vector2 WorldPosition { get; }
    public int GridX { get; }
    public int GridY { get; }
    
    public Node(Vector2 worldPosition, int gridX, int gridY, bool walkable)
    {
        WorldPosition = worldPosition;
        GridX = gridX;
        GridY = gridY;
        IsWalkable = walkable;
    }

    public int HeapIndex { get; set; }
    public Node Parent { get; set; }
    public bool IsWalkable { get; }

    public int CompareTo(Node other)
    {
        var comparison = FCost.CompareTo(other.FCost);

        if (comparison == 0)
            comparison = HCost.CompareTo(other.HCost);

        return -comparison;
    }
}