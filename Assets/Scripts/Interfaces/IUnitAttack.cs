using System;
using System.Collections;

public interface IUnitAttack
{
    event Action OnAttackFinished;
    event Action OnAttackStart;
    IEnumerator Attack();
}