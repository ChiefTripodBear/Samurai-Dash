using System;

public class GruntUnitMover : EnemyUnitMover
{
    public override event Action InheritancePathRequest;

    public GruntUnitMover(IUnitAttack unitAttack)
    {
        unitAttack.OnAttackStart += () => CanMoveThroughPath = false;
        unitAttack.OnAttackFinished += () =>
        {
            CanMoveThroughPath = true;
            InheritancePathRequest?.Invoke();
        };
    }
}