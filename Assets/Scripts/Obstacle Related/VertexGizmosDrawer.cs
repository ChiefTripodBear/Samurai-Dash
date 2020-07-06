using System.Collections.Generic;
using UnityEngine;

public class VertexGizmosDrawer
{
    public void DrawBoundaryPoints(List<ObstaclePoint> obstaclePoints)
    {
        foreach(var boundaryPoint in obstaclePoints)
        {
            Gizmos.DrawWireSphere(boundaryPoint.StartingPosition, .1f);
        }
    }
    
    public void DrawBoundaryPoints(List<ObstaclePoint> obstaclePoints, Color color)
    {
        foreach(var obstaclePoint in obstaclePoints)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(obstaclePoint.ProjectedPoint, .1f);
        }
    }

    public void DrawPoints(List<Vector2> points, Color color)
    {
        foreach (var point in points)
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(point, .1f);
        }
    }

    public void DrawPoint(Vector2 point, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(point, 0.05f);
    }

    public void DrawPairs(List<Vector2[]> pairs, Color color, Color colorSphere)
    {
        foreach (var pair in pairs)
        {
            // Gizmos.color = colorSphere;
            // Gizmos.DrawWireSphere(pair[0], 0.1f);
            // Gizmos.DrawWireSphere(pair[1], 0.1f);
            // Gizmos.color = color;

            Gizmos.DrawLine(pair[0], pair[1]);
        }
    }
    
    public void DrawPairs(Vector2 p1, Vector2 p2, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawLine(p1, p2);
    }

    public void DrawWayPointLines(List<ObstaclePoint> allWayPoints, Color color)
    {
        Gizmos.color = color;
        for (var i = 0; i < allWayPoints.Count; i++)
        {
            if (i == allWayPoints.Count - 1)
            {
                Gizmos.DrawLine(allWayPoints[allWayPoints.Count - 1].ProjectedPoint, allWayPoints[0].ProjectedPoint);
            }
            else
            {
                Gizmos.DrawLine(allWayPoints[i].ProjectedPoint, allWayPoints[i + 1].ProjectedPoint);
            }
        }
    }
}