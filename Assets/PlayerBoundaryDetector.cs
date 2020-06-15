using UnityEngine;

public static class PlayerBoundaryDetector
{
    private const float MoveStep = .1f;
    private const int OnScreenBuffer = 1;
    private static LayerMask UnwalkableLayer => LayerMask.GetMask("Unwalkable");
    private static Camera MainCam => Camera.main;
    
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

    public static bool WillCollideWithBoundaryAtTargetLocation(Vector2 targetLocation, Vector2 directionThroughTargetLocation, float scalar)
    {
        var projectedTargetLocation = targetLocation + directionThroughTargetLocation * scalar;

        return Physics2D.OverlapBox(projectedTargetLocation, Vector2.one, UnwalkableLayer);
    }

    private static Vector2 MoveTargetLocation(Vector2 negativeMoveDirection, Vector2 newPosition)
    {
        return newPosition + negativeMoveDirection * MoveStep;
    }

    public static bool OnScreen(Vector2 point)
    {
        var screenSize = new Vector2(MainCam.orthographicSize * MainCam.aspect, MainCam.orthographicSize);

        return point.x > -screenSize.x + OnScreenBuffer && point.x < screenSize.x - OnScreenBuffer && point.y > -screenSize.y + OnScreenBuffer && point.y < screenSize.y - OnScreenBuffer;
    }
}