using UnityEngine;

public class EnemyUnitMovementController
{
    private readonly UnitWayPointMover _wayPointMover;
    private readonly UnitEventSpecificMovements _unitEventSpecificMovements;
    private readonly UnitPathRequestManager _unitPathRequestManager;

    public UnitEventSpecificMovements UnitEventSpecificMovements => _unitEventSpecificMovements;
    
    public EnemyUnitMovementController(IUnitEnemy unitEnemy)
    {
        _unitPathRequestManager = new UnitPathRequestManager(unitEnemy);

        _wayPointMover = new UnitWayPointMover(unitEnemy);
        _wayPointMover.PathCompleted += RequestWayPointPath;
        RegisterPathRequester(_wayPointMover);
        
        _unitEventSpecificMovements = new UnitEventSpecificMovements(unitEnemy);
        _unitEventSpecificMovements.PathCompleted += RequestWayPointPath;
        RegisterPathRequester(_unitEventSpecificMovements);
        
        _unitEventSpecificMovements.PathCompleted += RequestWayPointPath;

        foreach (var pathRequestComponent in unitEnemy.MonoBehaviour.GetComponents<IPathRequester>())
        {
            RegisterPathRequester(pathRequestComponent);
            pathRequestComponent.PathCompleted += RequestWayPointPath;
        }
    }

    private void RequestWayPointPath()
    {
        _wayPointMover.RequestRingPath();
    }

    public void RequestSpawnPath(Vector2 destination)
    {
        _wayPointMover.RequestSpawnPath(destination);
    }

    private void RegisterPathRequester(IPathRequester pathRequester)
    {
        _unitPathRequestManager.RegisterRequester(pathRequester);
    }

    public void Tick()
    {
        _unitPathRequestManager.Tick();
        _unitEventSpecificMovements.TickFearTimer();
    }
}