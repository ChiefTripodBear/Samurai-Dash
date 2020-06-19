public class UnitPathRequestManager
{
    private readonly EnemyUnitMover _unitEnemyMover;
    private readonly IUnitEnemy _unitEnemy;
    private readonly PathRequestTracker _pathRequestTracker;
    
    public UnitPathRequestManager(IUnitEnemy unitEnemy)
    {
        _pathRequestTracker = new PathRequestTracker();
        _unitEnemy = unitEnemy;
        _unitEnemyMover = new EnemyUnitMover(_unitEnemy.MonoBehaviour);
        _unitEnemy.KillHandler.OnDeath += ClearQueue;
    }

    private void ClearQueue()
    {
        _pathRequestTracker.Clear();
        _unitEnemyMover.StopAllMovement();
    }
    
    public void Tick()
    {
        if (_pathRequestTracker.HasRequests() && _unitEnemyMover.CurrentRequest != null && 
            PathTypeComparison.PriorityLevel(_pathRequestTracker.PeekNextRequest().PathValues.PathType) <
            PathTypeComparison.PriorityLevel(_unitEnemyMover.CurrentRequest.PathValues.PathType))
        {
            _unitEnemyMover.FilterRequest(_pathRequestTracker.GetNextPathRequest());
            return;
        }

        if (PathManagementPaused()) return;
        
        _unitEnemyMover.FilterRequest(_pathRequestTracker.GetNextPathRequest());
    }

    private bool PathManagementPaused()
    {
        return !_unitEnemyMover.AvailableToFilterRequests() || !_pathRequestTracker.HasRequests() || _unitEnemy.Transform.gameObject.activeInHierarchy == false;
    }

    public void RegisterRequester(IPathRequester pathRequester)
    {
        pathRequester.PathRequested += EvaluateIncomingRequest;
    }

    private void EvaluateIncomingRequest(PathRequest pathRequest)
    {
        _pathRequestTracker.EnqueueRequest(pathRequest);
    }
}