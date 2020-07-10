using System;
using UnityEngine;

public class MovementPlanRequester
{
    private readonly MovementPlanner _movementPlanner;
    private Player _player;

    public MovementPlanRequester(Player player, PlayerMover playerMover)
    {
        _player = player;
        _movementPlanner = new MovementPlanner(_player, 5f);

        playerMover.MovementPlanRequested += RequestNextPlan;
    }

    private void RequestNextPlan(MovementPlan previousPlan, Action<MovementPlan> callback, bool redirected, bool movingThroughObstacle)
    {
        if (movingThroughObstacle)
        {
            callback(RequestPlanFromObstaclePath(previousPlan));
        }
        else
        {
            callback(redirected
                ? RequestPlanFromRedirectSuccess(previousPlan)
                : RequestPlanFromRedirectFailureOrFinish(previousPlan));
        }
    }

    public MovementPlan RequestStartPlan(Vector2 inputDirection, float moveAmount)
    {
        var unit = TargetDetector.GetValidUnitInFrontFromTargetPosition(null, moveAmount, inputDirection, _player.transform.position, 0.7f);

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