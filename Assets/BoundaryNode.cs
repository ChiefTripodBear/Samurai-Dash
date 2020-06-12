using UnityEngine;

public class BoundaryNode
{
    public Vector2 BoundaryNodePosition { get; }
    public BoundaryNode PreviousNeighbor { get; private set; }
    public BoundaryNode NextNeighbor { get; private set; }
    public int NodeIndex { get; }

    public BoundaryNode(Vector2 position, int nodeIndex)
    {
        NodeIndex = nodeIndex;
        BoundaryNodePosition = position;
    }


    public void SetNeighbors(BoundaryNode previousNeighbor, BoundaryNode nextNeighbor)
    {
        PreviousNeighbor = previousNeighbor;
        NextNeighbor = nextNeighbor;
    }
}