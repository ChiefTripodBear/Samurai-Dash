using System.Collections.Generic;
using UnityEngine;

public class ObstaclePoint
{
    public Vector2 StartingPosition;
    public Vector2 Normal;
    public int ObstaclePointIndex;
    public Vector2 Tangent;

    public bool NextIsCorner;
    public bool IsInsideCorner;
    public Vector2 ProjectionDirection;
    public Vector2 ProjectedPoint;
    public Vector2 ProjectedPointFrom;
    private List<ObstaclePoint> _boundaryPointsOnSide;
    private Color _randomDebugColor;
    public float CurrentScalar { get; private set; }
    public ObstaclePoint PreviousVertexPoint { get; set; }
    public ObstaclePoint NextVertexPoint { get; set; }
    public float PercentOfSegment { get; set; }
    public bool IsSidePoint { get; set; }
    public int WayPointIndex { get; set; }
    public float DistanceToNextWayPoint { get; set; }
    public float DistanceToPreviousWayPoint { get; set; }
    public ObstaclePoint PreviousWayPoint { get; set; }
    public ObstaclePoint NextWayPoint { get; set; }

    public void EvaluateProjectedPoint(float scalar)
    {
        CurrentScalar = scalar;
        ProjectedPoint = ProjectedPointFrom + ProjectionDirection * scalar;
    }

    public void SetVertexValues(Vector2 projectionDirection, Vector2 intersection)
    {
        ProjectionDirection = projectionDirection;
        ProjectedPointFrom = intersection;
    }
    
    public void SetSideProjection()
    {
        ProjectedPoint = Vector2.Lerp(PreviousVertexPoint.ProjectedPoint, NextVertexPoint.ProjectedPoint, PercentOfSegment);
    }
}