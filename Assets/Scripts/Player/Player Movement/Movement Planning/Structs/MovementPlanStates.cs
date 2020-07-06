public readonly struct MovementPlanStates
{
    public bool HeadingForIntersection { get; }
    public bool Finished { get; }
    public bool HeadingForObstacle { get; }

    public MovementPlanStates(bool headingForIntersection, bool finished, bool headingForObstacle)
    {
        HeadingForIntersection = headingForIntersection;
        Finished = finished;
        HeadingForObstacle = headingForObstacle;
    }
}