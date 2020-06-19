using System;
using UnityEngine;

public class PathRequest
{
    public PathValues PathValues { get; }
    public Action<bool> PathCompleteCallback { get; }
    private Func<Vector2[]> PathCreation { get; }
    private Func<Vector2?> DestinationCreation { get; }
    public IPathRequester PathRequester { get; }
    public IWaypoint WayPoint { get; }

    public PathRequest(IPathRequester pathRequester, PathValues pathValues, Func<Vector2[]> pathCreation, Action<bool> pathCompleteCallback)
    {
        PathCreation = pathCreation;
        PathValues = pathValues;
        PathRequester = pathRequester;
        PathCompleteCallback = pathCompleteCallback;
    }

    public PathRequest(IPathRequester pathRequester, PathValues pathValues, IWaypoint wayPoint, Func<Vector2[]> pathCreation,
        Action<bool> pathCompleteCallback)
    {
        PathCreation = pathCreation;
        PathValues = pathValues;
        PathRequester = pathRequester;
        PathCompleteCallback = pathCompleteCallback;
        WayPoint = wayPoint;
    }
    
    public PathRequest(IPathRequester pathRequester, PathValues pathValues, Func<Vector2?> destinationCreation, Action<bool> pathCompleteCallback)
    {
        DestinationCreation = destinationCreation;
        PathValues = pathValues;
        PathCompleteCallback = pathCompleteCallback;
        PathRequester = pathRequester;
    }
    
    public Vector2[] GeneratePath()
    {
        return PathCreation?.Invoke();
    }

    public Vector2? GenerateDestination()
    {
        return DestinationCreation?.Invoke();
    }
}