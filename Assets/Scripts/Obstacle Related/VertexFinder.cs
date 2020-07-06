using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VertexFinder : MonoBehaviour
{
    [SerializeField] private float _vertexWayPointScalar = 2f;
    [SerializeField] private float _minDistanceBetweenNewPointsBetweenVertices = 5f;

    private const float FixedStep = 0.1f;
    private const int MaxChecks = 1000;
    private const float OffsetDistance = 0.01f;
    
    private ObstaclePoint _startingPoint;
    private bool _shouldNotCheckForCorners;
    private int _pointIndex;
    
    private readonly List<Vector2[]> _debugTangent = new List<Vector2[]>();
    private readonly List<Vector2[]> _debugReverseNormals = new List<Vector2[]>();
    private readonly List<Vector2[]> _debugReverseTangents = new List<Vector2[]>();
    private readonly List<Vector2[]> _debugIntersections = new List<Vector2[]>();
    
    private readonly List<ObstaclePoint> _obstaclePoints = new List<ObstaclePoint>();
    private readonly List<ObstaclePoint> _vertices = new List<ObstaclePoint>();
    private readonly List<ObstaclePoint> _wayPoints = new List<ObstaclePoint>();

    public List<ObstaclePoint> Vertices => _vertices;
    public List<ObstaclePoint> WayPoints => _wayPoints;

    private VertexGizmosDrawer _vertexGizmosDrawer;

    private void Awake()
    { 
        _vertexGizmosDrawer = new VertexGizmosDrawer();
    }

    private void Update()
    {
        if (_wayPoints.Count > 0)
        {
            foreach (var wayPoint in _wayPoints)
            {
                if (wayPoint.IsSidePoint)
                {
                    wayPoint.SetSideProjection();
                }
                else
                {
                    wayPoint.EvaluateProjectedPoint(_vertexWayPointScalar);
                }
            }
        }
    }

    public List<ObstaclePoint> InitializeObstacle(Vector2 point, Vector2 normal, out List<Vector2> vertices)
    {
        ResetForNewDraw();

        FindNextPoint(point, normal);

        FindVertices();
        
        WeightWayPointsAlongSegments();

        SetDistancesBetweenPoints();

        vertices = _vertices.Select(t => t.ProjectedPoint).ToList();
        return _wayPoints;
    }

    private void ResetForNewDraw()
    {
        _pointIndex = 0;
        _startingPoint = null;
        _shouldNotCheckForCorners = false;
        _wayPoints.Clear();
        _obstaclePoints.Clear();
        _vertices.Clear();
        _debugIntersections.Clear();
        _debugTangent.Clear();
        _debugReverseNormals.Clear();
        _debugReverseTangents.Clear();
    }

    private void FindNextPoint(Vector2 point, Vector2 normal)
    {
        var tangent = (Vector2) Vector3.Cross(normal, Vector3.forward);
        var offsetPoint = point + normal * OffsetDistance;
        var tangentCheckPoint = offsetPoint + tangent * FixedStep;
        var reverseNormalCheckPoint = tangentCheckPoint - normal * (OffsetDistance + .005f);
        var reverseTangentCheckPoint = reverseNormalCheckPoint - tangent * (FixedStep * 2);

        var found = false;

        var obstaclePoint = new ObstaclePoint {StartingPosition = point, Normal = normal, ObstaclePointIndex = _pointIndex++, Tangent = tangent};
        if (_startingPoint == null)
            _startingPoint = obstaclePoint;

        if (_obstaclePoints.Count > 1)
        {
            if (Vector2.Distance(_startingPoint.StartingPosition, point) < FixedStep)
            {
                return;
            }
            
            if (_vertices.Count > 0)
            {
                if (Vector2.Distance(_startingPoint.StartingPosition, obstaclePoint.StartingPosition) <= FixedStep && _vertices[0].Normal == normal)
                {
                    _shouldNotCheckForCorners = true;
                }
            } 
            
            if (Vector2.Dot(_obstaclePoints[_obstaclePoints.Count - 1].Normal, normal) < 0.95f && !_shouldNotCheckForCorners)
            {
                if (_obstaclePoints[_obstaclePoints.Count - 1].NextIsCorner)
                {
                    obstaclePoint.IsInsideCorner = true;
                }
                _vertices.Add(obstaclePoint);
            }
        }
        
        _obstaclePoints.Add(obstaclePoint);
        
        var hit = Physics2D.Raycast(offsetPoint, tangent, FixedStep);

        if (hit)
        {
            obstaclePoint.NextIsCorner = true;
            _debugReverseTangents.Add(new []{offsetPoint, hit.point});
            found = true;
        }
        else
        {
            _debugReverseTangents.Add(new []{offsetPoint, tangentCheckPoint});

            hit = Physics2D.Raycast(tangentCheckPoint, -normal, OffsetDistance + FixedStep);

            if (hit)
            {
                _debugReverseNormals.Add(new []{tangentCheckPoint, hit.point});

                found = true;
            }
            else
            {
                _debugReverseTangents.Add(new []{tangentCheckPoint, reverseNormalCheckPoint});

                hit = Physics2D.Raycast(reverseNormalCheckPoint, -tangent, FixedStep * 2);

                if (hit)
                {
                    _debugReverseTangents.Add(new []{reverseNormalCheckPoint, hit.point});
                    found = true;
                }
                else
                {
                    _debugReverseTangents.Add(new []{reverseNormalCheckPoint, reverseTangentCheckPoint});
                }
            }
        }

        if (found && _obstaclePoints.Count < MaxChecks)
        {
            FindNextPoint(hit.point, hit.normal);
        }
    }

    private void FindVertices()
    {
        foreach (var vertex in _vertices)
        {
            var previousPoint = _obstaclePoints[(int) TrueMod(vertex.ObstaclePointIndex - 1, _obstaclePoints.Count)];

            var pointOne = vertex.StartingPosition;
            var pointTwo = vertex.IsInsideCorner
                ? pointOne + vertex.Normal * 0.3f
                : pointOne - vertex.Tangent * 0.3f;

            var pointThree = previousPoint.StartingPosition;
            var pointFour = vertex.IsInsideCorner
                ? pointThree + previousPoint.Normal * 0.3f
                : pointThree + previousPoint.Tangent * 0.3f;

            var intersection = IntersectionMaths.FindIntersection(pointOne, pointTwo, pointThree, pointFour);
            
            if (intersection.HasValue)
            {
                _debugIntersections.Add(new[] {pointOne, pointTwo});
                _debugIntersections.Add(new[] {pointThree, pointFour});

                var intersectProjected = (pointTwo + pointFour) / 2.0f;

                vertex.SetVertexValues((intersectProjected - intersection.Value).normalized, intersection.Value);
                vertex.EvaluateProjectedPoint(_vertexWayPointScalar);
            }
            else
            {
                if (vertex.IsInsideCorner)
                {
                    var previousOffset = previousPoint.StartingPosition + previousPoint.Normal * .001f;
                    var previousPointHit = Physics2D.Raycast(previousOffset, previousPoint.Tangent);

                    var previousPointDistance = Vector2.Distance(previousPointHit.point, previousOffset);

                    var cornerOffset = vertex.StartingPosition + vertex.Normal * .001f;
                    var cornerHit = Physics2D.Raycast(cornerOffset, -vertex.Tangent);


                    var cornerPointDistance = Vector2.Distance(cornerHit.point, cornerOffset);

                    var moveDistance = 0f;

                    if (cornerPointDistance < previousPointDistance)
                    {
                        moveDistance = previousPointDistance - cornerPointDistance;

                        vertex.StartingPosition += vertex.Tangent * moveDistance;
                    }
                    else
                    {
                        moveDistance = cornerPointDistance - previousPointDistance;

                        previousPoint.StartingPosition -= previousPoint.Tangent * moveDistance;
                    }

                    pointOne = vertex.StartingPosition;
                    pointTwo = pointOne + vertex.Normal * 0.3f;
                    pointThree = previousPoint.StartingPosition;
                    pointFour = pointThree + previousPoint.Normal * 0.3f;

                    intersection = IntersectionMaths.FindIntersection(pointOne, pointTwo, pointThree, pointFour);


                    if (intersection.HasValue)
                    {
                        _debugIntersections.Add(new[] {pointOne, pointTwo});
                        _debugIntersections.Add(new[] {pointThree, pointFour});

                        var intersectProjected = (pointTwo + pointFour) / 2.0f;

                        vertex.SetVertexValues((intersectProjected - intersection.Value).normalized, intersection.Value);
                        vertex.EvaluateProjectedPoint(_vertexWayPointScalar);
                    }
                }
            }
        }
    }

    private void WeightWayPointsAlongSegments()
    {
        for (var i = 0; i < _vertices.Count; i++)
        {
            var firstVertex = _vertices[i];
            var nextVertex = i == _vertices.Count - 1 ? _vertices[0] : _vertices[i + 1];

            _wayPoints.Add(firstVertex);

            var distanceBetween = Vector2.Distance(firstVertex.ProjectedPoint, nextVertex.ProjectedPoint);

            var pointsToAdd = Mathf.FloorToInt(distanceBetween / _minDistanceBetweenNewPointsBetweenVertices) <= 0
                ? 1
                : Mathf.FloorToInt(distanceBetween / _minDistanceBetweenNewPointsBetweenVertices);

            if (pointsToAdd == 1)
            {
                var obstaclePoint = new ObstaclePoint
                {
                    IsSidePoint = true,
                    PreviousVertexPoint = firstVertex,
                    NextVertexPoint = nextVertex,
                    PercentOfSegment = 0.5f,
                    ProjectedPoint = Vector2.Lerp(firstVertex.ProjectedPoint, nextVertex.ProjectedPoint, 0.5f)
                };

                _wayPoints.Add(obstaclePoint);
            }
            else
            {
                for (var j = 0; j < pointsToAdd - 1; j++)
                {
                    var percentOfSegment = (j + 1) / (float) pointsToAdd;

                    var pointPosition =
                        Vector2.Lerp(firstVertex.ProjectedPoint, nextVertex.ProjectedPoint, percentOfSegment);

                    var projectedPointFrom = pointPosition;
                    var projectionDirection = firstVertex.Normal;
                    var obstaclePoint = new ObstaclePoint
                    {
                        IsSidePoint = true,
                        StartingPosition = pointPosition,
                        ProjectedPointFrom = projectedPointFrom,
                        ProjectionDirection = projectionDirection,
                        ProjectedPoint = pointPosition,
                        NextVertexPoint = nextVertex,
                        PreviousVertexPoint = firstVertex,
                        PercentOfSegment = percentOfSegment
                    };

                    _wayPoints.Add(obstaclePoint);
                }
            }
        }
    }

    private void SetDistancesBetweenPoints()
    {
        for (var i = 0; i < _wayPoints.Count; i++)
        {
            _wayPoints[i].WayPointIndex = i;
            var previousPoint = i == 0 ? _wayPoints[_wayPoints.Count - 1] : _wayPoints[i - 1]; 
            var firstPoint = _wayPoints[i];
            var nextPoint = i == _wayPoints.Count - 1 ? _wayPoints[0] : _wayPoints[i + 1];

            _wayPoints[i].PreviousWayPoint = previousPoint;
            _wayPoints[i].NextWayPoint = nextPoint;
            var forwardDistance = Vector2.Distance(firstPoint.ProjectedPoint, nextPoint.ProjectedPoint);
            var reverseDistance = Vector2.Distance(firstPoint.ProjectedPoint, previousPoint.ProjectedPoint);
            
            firstPoint.DistanceToNextWayPoint = forwardDistance;
            firstPoint.DistanceToPreviousWayPoint = reverseDistance;
        }
    }

    private void OnDrawGizmos()
    { 
        // _vertexGizmosDrawer.DrawPairs(_debugTangent, Color.red, Color.red);
        // _vertexGizmosDrawer.DrawPairs(_debugReverseNormals, Color.green, Color.green);
        // _vertexGizmosDrawer.DrawPairs(_debugReverseTangents, Color.cyan, Color.cyan);
        // _vertexGizmosDrawer.DrawPairs(_debugIntersections, Color.magenta, Color.yellow);
        // _vertexGizmosDrawer.DrawBoundaryPoints(_boundaryPoints);

        if (_wayPoints == null || _vertexGizmosDrawer == null) return;
        
        _vertexGizmosDrawer.DrawBoundaryPoints(_wayPoints, Color.red);
        _vertexGizmosDrawer.DrawWayPointLines(_wayPoints, Color.red);
    }
    
    private static float TrueMod(float a, float b)
    {
        return a - b * Mathf.Floor(a / b);
    }
}