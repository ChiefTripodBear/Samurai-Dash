using System.Collections.Generic;
using System.Linq;

public class PathRequestTracker
{
    private readonly Queue<PathRequest> _pathRequests = new  Queue<PathRequest>();
    
    public void EnqueueRequest(PathRequest pathRequest)
    {
        if (_pathRequests.Any(t => t.PathValues.PathType == pathRequest.PathValues.PathType)) return;
        
        _pathRequests.Enqueue(pathRequest);
        SortRequests();
    }

    public PathRequest GetNextPathRequest()
    {
        return _pathRequests.Count > 0 ? _pathRequests.Dequeue() : null;
    }

    private void SortRequests()
    {
        var pathRequestsTempList = _pathRequests.ToList();

        for (var i = 0; i < pathRequestsTempList.Count - 1; i++)
        {
            var swapped = false;
            for (var j = 0; j < pathRequestsTempList.Count - i - 1; j++)
                if (PathTypeComparison.PriorityLevel(pathRequestsTempList[i].PathValues.PathType) >
                    PathTypeComparison.PriorityLevel(pathRequestsTempList[j + 1].PathValues.PathType))
                {
                    var temp = pathRequestsTempList[j];
                    pathRequestsTempList[j] = pathRequestsTempList[j + 1];
                    pathRequestsTempList[j + 1] = temp;
                    swapped = true;
                }

            if (!swapped)
                break;
        }

        _pathRequests.Clear();
        
        foreach (var request in pathRequestsTempList) _pathRequests.Enqueue(request);
    }

    public bool HasRequests()
    {
        return _pathRequests.Count > 0;
    }

    public void Clear()
    {
        _pathRequests.Clear();
    }

    public PathRequest PeekNextRequest()
    {
        return _pathRequests.Peek();
    }
}