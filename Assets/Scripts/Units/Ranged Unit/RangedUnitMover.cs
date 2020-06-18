using System;

public class RangedUnitMover : EnemyUnitMover
{
    public override event Action InheritancePathRequest;

    public RangedUnitMover(IUnitAttack unitAttack)
    {
        unitAttack.OnAttackStart += () => CanMoveThroughPath = false;
        unitAttack.OnAttackFinished += () =>
        {
            CanMoveThroughPath = true;
            InheritancePathRequest?.Invoke();
        };
    }
}