using UnityEngine;

public static class PlayerBoundaryDetector
{
    private const float MoveStep = .1f;
    private static LayerMask UnwalkableLayer => LayerMask.GetMask("Unwalkable");
    
    public static Vector2 HandleBoundaryCollision(Vector2 targetLocation, Vector2 moveDirection)
    {
        var willCollideWithBoundary = WillCollideWithBoundaryAtTargetLocation(targetLocation);
        
        var newPosition = targetLocation;

        if (!willCollideWithBoundary) return newPosition;
        
        while (willCollideWithBoundary)
        {
            newPosition = MoveTargetLocation(-moveDirection, newPosition); 
            willCollideWithBoundary = Physics2D.OverlapBox(newPosition, Vector2.one, 0, UnwalkableLayer);
        }

        return newPosition;
    }

    public static bool WillBeMovingThroughBoundary(Vector2 startPosition, Vector2 targetLocation, out Boundary boundary)
    {
        var direction = (targetLocation - startPosition).normalized;
        var distance = Vector2.Distance(targetLocation, startPosition);
        var hit = Physics2D.Raycast(startPosition, direction, distance, UnwalkableLayer);
        
        boundary = hit ? hit.collider.GetComponent<Boundary>() : null;
        
        return hit && !WillCollideWithBoundaryAtTargetLocation(targetLocation);
    }

    public static bool WillCollideWithBoundaryAtTargetLocation(Vector2 targetLocation)
    {
        return Physics2D.OverlapBox(targetLocation, Vector2.one, 0, UnwalkableLayer);
    }

    private static Vector2 MoveTargetLocation(Vector2 negativeMoveDirection, Vector2 newPosition)
    {
        return newPosition + negativeMoveDirection * MoveStep;
    }
}