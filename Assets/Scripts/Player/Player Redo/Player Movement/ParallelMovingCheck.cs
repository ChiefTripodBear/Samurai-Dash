using UnityEngine;

public class ParallelMovingCheck
{
    private readonly Transform _mover;

    private Collider2D[] _potentialParallelUnits = new Collider2D[20];
    
    public ParallelMovingCheck(Transform mover)
    {
        _mover = mover;
    }
    
    public void Tick(Vector2 moveDirection)
    {
        var colliderCount = Physics2D.OverlapCircleNonAlloc(_mover.position, .5f, _potentialParallelUnits);

        for (var i = 0; i < colliderCount; i++)
        {
            var unit = _potentialParallelUnits[i].GetComponent<IUnit>();
            
            if(unit == null) continue;

            if (!TargetDetector.DotProductSuccess(unit, moveDirection)) continue;
            
            unit.KillHandler.SetKillPoint();
        }
    }
}