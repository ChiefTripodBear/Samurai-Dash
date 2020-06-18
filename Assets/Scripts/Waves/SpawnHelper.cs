using UnityEngine;

public class SpawnHelper : MonoBehaviour
{
    private static SpawnHelper _instance;
    public static SpawnHelper Instance => _instance;
    
    private void Awake()
    {
        if (_instance == null) _instance = this;
    }

    public Vector2 ValidPointOnScreen()
    {
        return FindBestNode();
    }

    private Vector2 FindBestNode()
    {
        var position = NodeGrid.Instance.GetRandomWalkableNode().WorldPosition;
        return position;
    }
    
    public Vector2 ValidPointOnScreenXDistanceFromTarget(Vector2 targetPosition, float distance)
    {
        return FindBestNodeGivenDistance(targetPosition, distance);
    }

    private Vector2 FindBestNodeGivenDistance(Vector2 targetPosition, float distance)
    {
        return NodeGrid.Instance.GetRandomWalkableNodeXUnitsFromTarget(distance, targetPosition).WorldPosition;
    }
}