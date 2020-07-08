using System.Collections.Generic;
using UnityEngine;

public static class ObstacleHelper
{
    private static List<Vector2> _adjustedPositions = new List<Vector2>();
    private static LayerMask ObstacleMask => LayerMask.GetMask("Unwalkable");

    public static ObstacleObject GetObstacle(Vector2 checkFromPosition, Vector2 checkToPosition)
    {
        var adjustedPosition = checkFromPosition;
        var distance = Vector2.Distance(adjustedPosition, checkToPosition);
        var direction = (checkToPosition - checkFromPosition).normalized;
        
        while (distance > 0.1f)
        {
            _adjustedPositions.Add(adjustedPosition);
            distance = Vector2.Distance(adjustedPosition, checkToPosition);

            var collider = Physics2D.OverlapBox(adjustedPosition, Vector2.one, 0, ObstacleMask);

            adjustedPosition += direction * 0.1f;
            
            if (collider == null)
                continue;

            var obstacleObject = collider.GetComponent<ObstacleObject>();

            if (obstacleObject != null)
                return obstacleObject;
        }
        
        return null;
    }

    public static void DrawObstacleCheckBoxes()
    {
        foreach(var adjustedPosition in _adjustedPositions)
            Gizmos.DrawWireCube(adjustedPosition, Vector2.one);
    }
}