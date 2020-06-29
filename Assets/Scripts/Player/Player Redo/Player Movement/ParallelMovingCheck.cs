using UnityEngine;

public class ParallelMovingCheck
{
    private readonly Transform _transformMover;
    private readonly Mover _mover;

    private Collider2D[] _potentialParallelUnits = new Collider2D[20];
    
    public ParallelMovingCheck(Transform transformMover, Mover mover)
    {
        _transformMover = transformMover;
        _mover = mover;
    }
    
    public void Tick(Vector2 moveDirection)
    {
        var colliderCount = Physics2D.OverlapCircleNonAlloc(_transformMover.position, .5f, _potentialParallelUnits);

        for (var i = 0; i < colliderCount; i++)
        {
            var unit = _potentialParallelUnits[i].GetComponent<IUnit>();
            
            if(unit == null || unit == _mover.MovementPackage.Destination?.Unit || unit == _mover.MovementPackage.Destination?.PreviousIntersectingUnit) continue;

            if (!TargetDetector.DotProductSuccess(unit, moveDirection, .98f)) continue;
            
            unit.KillHandler.SetKillPoint();
        }
    }
}