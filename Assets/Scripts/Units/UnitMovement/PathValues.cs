public struct PathValues
{
    public float MoveSpeed { get; }
    public float PrePathWaitTime { get; }
    public float PostPathWaitTime { get; }
    public PathType PathType { get; }

    public PathValues(float moveSpeed, float prePathWaitTime, float postPathWaitTime, PathType pathType)
    {
        MoveSpeed = moveSpeed;
        PrePathWaitTime = prePathWaitTime;
        PostPathWaitTime = postPathWaitTime;
        PathType = pathType;
    }
}