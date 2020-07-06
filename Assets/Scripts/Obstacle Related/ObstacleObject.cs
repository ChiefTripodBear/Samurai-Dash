using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObstacleObject : MonoBehaviour
{
    [SerializeField] private Transform _startingPoint;

    [SerializeField] private Transform _destinationTest;
    private VertexFinder _vertexFinder;
    private List<ObstaclePoint> _wayPoints;
    private Queue<ObstaclePoint> _selectedPath;
    public float AverageDistanceBetweenPoints { get; private set; }
    
    private List<Vector2> _vertices = new List<Vector2>();
    private List<Vector2[]> _segments = new List<Vector2[]>();
    private List<Vector2> _closestPoints = new List<Vector2>();

    private void Awake()
    {
        _vertexFinder = GetComponent<VertexFinder>();
    }

    private void Start()
    {
        SetWayPoints();

        SetSegments();

        // BestPath(_destinationTest.position, _startingPoint.position);

        AverageDistanceBetweenPoints = _wayPoints.Average(t => t.DistanceToNextWayPoint);
    }
    
    private void SetWayPoints()
    {
        var hit = Physics2D.Raycast(_startingPoint.position, Vector2.up);

        _wayPoints =_vertexFinder.InitializeObstacle(hit.point, hit.normal, out _vertices);
    }

    private void SetSegments()
    {
        for (var i = 0; i < _vertices.Count; i++)
        {
            var firstPoint = _vertices[i];
            var nextPoint = i == _vertices.Count - 1 ? _vertices[0] : _vertices[i + 1];
            
            _segments.Add(new []{firstPoint, nextPoint});
        }
    }

    public ObstaclePoint FindClosestPointOnSegmentFromProjectedPoint(Vector2 projectedPoint)
    {
        _closestPoints.Add(projectedPoint);
        foreach (var segment in _segments)
        {
            if (!IsBetween(segment[0], projectedPoint, segment[1])) continue;
            
            var closestPoint = _wayPoints.OrderBy(t => Vector2.Distance(projectedPoint, t.ProjectedPoint))
                .FirstOrDefault();

            if (closestPoint != null)
                return closestPoint;
        }
        
        return null;
    }

    private bool IsBetween(Vector2 a, Vector2 c, Vector2 b)
    {
        var cross = (c.y - a.y) * (b.x - a.x) - (c.x - a.x) * (b.y - a.y);

        if (Mathf.Abs(cross) > 3f)
            return false;

        var dot = (c.x - a.x) * (b.x - a.x) + (c.y - a.y) * (b.y - a.y);

        if (dot < 0)
            return false;

        var squaredLengthba = (b.x - a.x) * (b.x - a.x) + (b.y - a.y) * (b.y - a.y);

        if (dot > squaredLengthba)
            return false;

        return true;
    }

    public Queue<ObstaclePoint> BestPath(ObstaclePoint closestToStart, ObstaclePoint closestToEnd, Vector2 destination)
    {
        var forwardPath = new Queue<ObstaclePoint>();
        var reversePath = new Queue<ObstaclePoint>();
        
        var closestPointToDestinationIndex = closestToEnd.WayPointIndex;

        var currentObstaclePoint = closestToStart;
        forwardPath.Enqueue(currentObstaclePoint);

        var forwardDistanceSum = 0f;

        while (currentObstaclePoint.WayPointIndex != closestPointToDestinationIndex)
        {
            forwardDistanceSum += currentObstaclePoint.DistanceToNextWayPoint;
            
            currentObstaclePoint = currentObstaclePoint.NextWayPoint;
            forwardPath.Enqueue(currentObstaclePoint);
        }
        
        var closestToFinalDestination = Vector2.Distance(closestToEnd.ProjectedPoint, destination);
        
        currentObstaclePoint = closestToStart;
        reversePath.Enqueue(currentObstaclePoint);
        var reverseDistanceSum = 0f;

        while (currentObstaclePoint.WayPointIndex != closestPointToDestinationIndex)
        {
            reverseDistanceSum += currentObstaclePoint.DistanceToPreviousWayPoint;
            
            currentObstaclePoint = currentObstaclePoint.PreviousWayPoint;
            reversePath.Enqueue(currentObstaclePoint);
        }
        
        forwardDistanceSum += closestToFinalDestination;
        reverseDistanceSum += closestToFinalDestination;

        var selectedPath = forwardDistanceSum < reverseDistanceSum ? forwardPath : reversePath;

        _selectedPath = selectedPath;
        
        return selectedPath;
    }

    private void OnDrawGizmos()
    {
        if (_selectedPath != null)
        {
            foreach (var point in _selectedPath)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(point.ProjectedPoint, 0.1f);
                Gizmos.DrawLine(point.ProjectedPoint, point.NextWayPoint.ProjectedPoint);
            }
        }

        if (_closestPoints.Count <= 0) return;

        // foreach (var point in _closestPoints)
        // {
        //     Gizmos.DrawWireSphere(point, 0.1f);
        // }
    }
}