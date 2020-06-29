using System.Collections;
using UnityEngine;

public class EnemyUnitMover
{
    private int _pathIndex;
    private PathRequest _currentRequest;
    private Coroutine _currentMovementRoutine;
    private readonly MonoBehaviour _monoBehaviour;
    private bool _isProcessingRequest;
    private bool _isCurrentlyMoving;
    private Vector2[] _path;
    
    public PathRequest CurrentRequest => _currentRequest;

    public EnemyUnitMover(MonoBehaviour monoBehaviour)
    {
        _monoBehaviour = monoBehaviour;
    }
    
    public void StopAllMovement()
    {
        if (_currentMovementRoutine != null)
        {
            _monoBehaviour.StopCoroutine(_currentMovementRoutine);
            _currentRequest = null;
            _isCurrentlyMoving = false;
            _isProcessingRequest = false;    
        }
    }

    public bool AvailableToFilterRequests()
    {
        return !_isCurrentlyMoving && !_isProcessingRequest;
    }

    public void FilterRequest(PathRequest nextRequest)
    {
        _pathIndex = 0;
        _isProcessingRequest = true;
        
        if (_currentMovementRoutine != null)
        {
            _currentRequest?.PathCompleteCallback?.Invoke(false);
            _monoBehaviour.StopCoroutine(_currentMovementRoutine);
        }
        _currentRequest = nextRequest;
        
        var generatedPath = _currentRequest.GeneratePath();
        _path = generatedPath;
        var generatedDestination = _currentRequest.GenerateDestination();
        
        if (generatedPath != null)
            _currentMovementRoutine = _currentRequest.WayPoint != null
                ? _monoBehaviour.StartCoroutine(WayPointPath(_currentRequest, generatedPath))
                : _monoBehaviour.StartCoroutine(NoWayPointPath(_currentRequest, generatedPath));
        else
            _currentMovementRoutine = _monoBehaviour.StartCoroutine(NoPathNoWayPoint(_currentRequest, generatedDestination));

        _isProcessingRequest = false;
    }

    private IEnumerator NoWayPointPath(PathRequest request, Vector2[] generatedPath)
    {
        _isCurrentlyMoving = true;

        yield return new WaitForSeconds(request.PathValues.PrePathWaitTime);

        while (true)
        {
            if (FinishedPath(generatedPath.Length))
            {
                yield return new WaitForSeconds(request.PathValues.PostPathWaitTime);
                _currentRequest = null;
                _isCurrentlyMoving = false;
                _pathIndex = 0;
                request.PathCompleteCallback?.Invoke(true);
                yield break;
            }
            
            var currentWayPoint = generatedPath[_pathIndex];
            
            if (ArrivedAtPoint(request.PathRequester.Mover, currentWayPoint))
                _pathIndex++;
            else
                Move(request.PathRequester.Mover, request.PathValues.MoveSpeed, currentWayPoint);

            yield return null;
        }
    }

    private IEnumerator WayPointPath(PathRequest request, Vector2[] generatedPath)
    {
        var startingDestinationPosition = request.WayPoint.Transform.position;
        _isCurrentlyMoving = true;

        yield return new WaitForSeconds(request.PathValues.PrePathWaitTime);

        while (true)
        {
            if (FinishedPath(generatedPath.Length))
            {
                yield return new WaitForSeconds(request.PathValues.PostPathWaitTime);
                _currentRequest = null;
                request.PathCompleteCallback?.Invoke(true);
                _isCurrentlyMoving = false;
                yield break;
            }
            
            var currentWayPoint = generatedPath[_pathIndex];
            
            if (ArrivedAtPoint(request.PathRequester.Mover, currentWayPoint))
            {
                _pathIndex++;

                if (TargetMoved(startingDestinationPosition, request.WayPoint.Transform.position))
                {
                    _currentRequest = null;
                    request.PathCompleteCallback?.Invoke(true);
                    _isCurrentlyMoving = false;
                    yield break;
                }
            }
            else
            {
                Move(request.PathRequester.Mover, request.PathValues.MoveSpeed, currentWayPoint);
            }
            
            yield return null;
        }
    }

    private IEnumerator NoPathNoWayPoint(PathRequest request, Vector2? generatedDestination)
    {
        if (!generatedDestination.HasValue)
        {
            _isCurrentlyMoving = false;
            request.PathCompleteCallback?.Invoke(false);
            _currentRequest = null;
            yield break;
        }
        
        _isCurrentlyMoving = true;

        yield return new WaitForSeconds(request.PathValues.PrePathWaitTime);

        while (!ArrivedAtPoint(request.PathRequester.Mover, generatedDestination.Value))
        {
            Move(request.PathRequester.Mover, request.PathValues.MoveSpeed, generatedDestination.Value);
            yield return null;
        }

        yield return new WaitForSeconds(request.PathValues.PostPathWaitTime);
        request.PathCompleteCallback?.Invoke(true);
        _currentRequest = null;
        _isCurrentlyMoving = false;
    }
    
    private bool FinishedPath(int pathCount) => _pathIndex >= pathCount;

    private static bool ArrivedAtPoint(Transform mover, Vector2 currentWayPoint) 
        => Vector2.Distance(mover.transform.position, currentWayPoint) < 0.1f;

    private static void Move(Transform mover, float moveSpeed, Vector2 currentWayPoint) 
        => mover.position = Vector2.MoveTowards(mover.position, currentWayPoint, moveSpeed * Time.deltaTime);

    private static bool TargetMoved(Vector2 startingPosition, Vector2 currentDestination) 
        => Vector2.Distance(startingPosition, currentDestination) > 2f;


    public void DrawGizmos()
    {
        if (_path != null)
        {
            for (int i = 0; i < _path.Length ; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(_path[i], Vector3.one);
            }
        }
    }
}
