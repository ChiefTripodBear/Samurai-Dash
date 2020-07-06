using System;
using UnityEngine;

public class MovementPlanRequester
{
    private readonly Transform _transform;
    private readonly MovementPlanner _movementPlanner;

    public MovementPlanRequester(Transform transform)
    {
        _transform = transform;
        _movementPlanner = new MovementPlanner(transform, 5f);

        PlayerMover.MovementPlanRequested += RequestNextPlan;
    }
    
    private void RequestNextPlan(MovementPlan previousPlanner, Action<MovementPlan> callback, bool redirected, bool movingThroughObstacle)
    {
        if (movingThroughObstacle)
        {
            callback(RequestPlanFromObstaclePath(previousPlanner));
        }
        else
        {
            callback(redirected
                ? RequestPlanFromRedirectSuccess(previousPlanner)
                : RequestPlanFromRedirectFailureOrFinish(previousPlanner));
        }
    }

    public MovementPlan RequestStartPlan(Vector2 inputDirection, float moveAmount)
    {
        var unit = TargetDetector.GetValidUnitInFrontFromTargetPosition(null, moveAmount, inputDirection, _transform.position, 0.7f);

        return _movementPlanner.GetStartingPlan(inputDirection, unit);
    }

    private MovementPlan RequestPlanFromRedirectFailureOrFinish(MovementPlan previousPlan)
    {
        return _movementPlanner.FinishPlanOrMoveToNextIntersect(previousPlan);
    }

    private MovementPlan RequestPlanFromRedirectSuccess(MovementPlan previousPlan)
    {
        return _movementPlanner.PlanMovingTowardsIntersectingUnitFromPreviousIntersection(previousPlan);
    }
    
    private MovementPlan RequestPlanFromObstaclePath(MovementPlan previousPlan)
    {
        return _movementPlanner.GetPlanForNextMoveDuringObstaclePath(previousPlan);
    }
}