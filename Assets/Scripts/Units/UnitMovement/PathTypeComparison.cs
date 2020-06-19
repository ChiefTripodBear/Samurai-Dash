using System.Collections.Generic;

public static class PathTypeComparison
{
    private static readonly Dictionary<PathType, int> _pathPriorities;
    
    static PathTypeComparison()
    {
        _pathPriorities = new Dictionary<PathType, int>
        {
            {PathType.Attack, 2},
            {PathType.Ring, 3},
            {PathType.Spawn, 0},
            {PathType.EventBased, 1}
        };
    }

    public static int PriorityLevel(PathType pathType)
    {
        return _pathPriorities[pathType];
    }
}