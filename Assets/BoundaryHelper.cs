using System;
using UnityEngine;

public static class BoundaryHelper
{
    private const float MoveStep = .1f;
    private const int OnScreenBuffer = 1;
    private const float AngleAdjustmentStep = 10;
    private static LayerMask UnwalkableLayer => LayerMask.GetMask("Unwalkable");
    private static Camera MainCam => Camera.main;
    
    public static Vector2 HandleBoundaryCollision(Vector2 targetLocation, Vector2 moveDirection, out bool requiredCollisionHandling)
    {
        var willCollideWithBoundary = WillCollideWithBoundaryAtTargetLocation(targetLocation);
        
        var newPosition = targetLocation;

        if (!willCollideWithBoundary)
        {
            requiredCollisionHandling = false;
            return newPosition;
        }
        
        while (willCollideWithBoundary)
        {
            newPosition = MoveTargetLocation(-moveDirection, newPosition); 
            willCollideWithBoundary = Physics2D.OverlapBox(newPosition, Vector2.one, 0, UnwalkableLayer);
        }

        requiredCollisionHandling = newPosition != targetLocation;

        return newPosition;
    }

    public static bool WillBeMovingThroughBoundary(Vector2 startPosition, Vector2 targetLocation, out Boundary boundary)
    {
        var hit = GetUnwalkableRaycastHit2D(startPosition, targetLocation);

        boundary = hit ? hit.collider.GetComponent<Boundary>() : null;
        
        return hit && !WillCollideWithBoundaryAtTargetLocation(targetLocation);
    }

    private static RaycastHit2D GetUnwalkableRaycastHit2D(Vector2 startPosition, Vector2 targetLocation)
    {
        var direction = (targetLocation - startPosition).normalized;
        var distance = Vector2.Distance(targetLocation, startPosition);
        var hit = Physics2D.Raycast(startPosition, direction, distance, UnwalkableLayer);
        return hit;
    }

    public static Vector2 PointBeforeCollisionWithBoundaryWithBuffer(Vector2 startPosition, Vector2 targetLocation, float bufferScalar)
    {
        var direction = (targetLocation - startPosition).normalized;
        var hit = GetUnwalkableRaycastHit2D(startPosition, targetLocation);

        return hit ? hit.point + -direction * bufferScalar : targetLocation;
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

    public static bool TargetDestinationPathLeadsIntoUnwalkable(Vector2 targetLocation, Vector2 startingPoint)
    {
        var hit = GetUnwalkableRaycastHit2D(startingPoint, targetLocation);

        return WillCollideWithBoundaryAtTargetLocation(targetLocation) ? hit : false;
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
    
    public static Vector2 AdjustDirectionToFindValidPosition(Vector2 fromPosition, float scalar, Vector2 targetPosition)
    {
        var invalid = WillCollideWithBoundaryAtTargetLocation(targetPosition);

        var direction = (targetPosition - fromPosition).normalized;
        
        var angle = Vector2.SignedAngle(direction, Vector2.up);

        var startingAngle = angle;
        while (invalid)
        {
            angle += AngleAdjustmentStep;

            if (Math.Abs(angle - 360 - startingAngle) < .5f)
            {
                targetPosition = fromPosition;
                break;
            }
            
            targetPosition = fromPosition +
                                    new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad)) *
                                    scalar;
            
            invalid = WillCollideWithBoundaryAtTargetLocation(targetPosition);
        }

        return targetPosition;
    }
}