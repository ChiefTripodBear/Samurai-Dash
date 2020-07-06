using UnityEngine;

public class Destination
{
    public Vector2 TargetLocation;
    public Vector2 MoveDirection;
    public IUnit Unit;
    public DestinationType DestinationType;
    public IUnit PreviousIntersectingUnit;

    public void Initialize(Vector2 targetLocation, Vector2 moveDirection, IUnit unit)
    {
        TargetLocation = targetLocation;
        MoveDirection = moveDirection;
        Unit = unit;
    }
}