public class UnitEventSpecificMovements
{
    private float _fearTime = 1f;
    
    private readonly IUnitEnemy _unitEnemy;
    private readonly Player _player;

    public UnitEventSpecificMovements(IUnitEnemy unitEnemy, Player player)
    {
        _unitEnemy = unitEnemy;
        _player = player;
    }

    public void PerformFear()
    {
        var fearDirection = (_unitEnemy.Transform.position - _player.transform.position).normalized;
        var targetPosition =  _unitEnemy.Transform.position + fearDirection * 2f; 
        _unitEnemy.UnitMovementManager.MoveForTime(targetPosition, _unitEnemy.Transform,  2f, _fearTime);
    }
}