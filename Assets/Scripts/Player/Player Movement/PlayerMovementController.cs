using UnityEngine;

public class PlayerMovementController
{
    private readonly Player _player;
    private readonly InputProcessor _inputProcessor;
    private readonly MovementPlanRequester _movementPlanRequester;
    private readonly PlayerMover _playerMover;
    private readonly ParallelMovingCheck _parallelMovingCheck;
    private readonly PlayerDashMonitor _playerDashMonitor;

    private Vector2? _startPosition;
    private Vector2? _endPosition;
    
    public PlayerMovementController(Player player)
    {
        _player = player;
        _inputProcessor = new InputProcessor();
        var transform = player.transform;
        _movementPlanRequester = new MovementPlanRequester(transform);
        _playerMover = new PlayerMover(transform);
        _parallelMovingCheck = new ParallelMovingCheck(transform, _playerMover);
        _playerDashMonitor = new PlayerDashMonitor();
        _playerSafetyCheck = new PlayerSafetyRedirect(_playerMover, player);
    }

    private IUnit _currentUnit;
    private PlayerSafetyRedirect _playerSafetyCheck;

    public void Tick()
    {
        if (!_playerMover.IsMoving) _playerDashMonitor.RefundChargesWhileNotMoving();
        
        // _playerSafetyCheck.Tick();
        
        if (_playerMover.CurrentPlan?.TargetUnit != null)
        {
            if (_currentUnit != null) _currentUnit.Transform.GetComponent<SpriteRenderer>().color = Color.white;
            _currentUnit = _playerMover.CurrentPlan.TargetUnit;
            _currentUnit.Transform.GetComponent<SpriteRenderer>().color = Color.magenta;
        }
        _playerMover.Tick();
        _parallelMovingCheck.Tick();

        if (_playerMover.IsMoving || _playerDashMonitor.CurrentCharges <= 0) return;

        var startingDirection = InputDestination();

        if (!startingDirection.HasValue || startingDirection == Vector2.zero) return;

        _playerMover.SetCurrentPlan(_movementPlanRequester.RequestStartPlan(startingDirection.Value, 5f));
    }
    
    private Vector2? InputDestination()
    {
        if (_inputProcessor.MouseDown) _startPosition = _inputProcessor.MousePosition();

        if (_inputProcessor.MouseUp && _startPosition.HasValue) _endPosition = _inputProcessor.MousePosition();

        if (_startPosition.HasValue && _endPosition.HasValue)
        {
            var startingDirection = _inputProcessor.GetMoveDirection(_startPosition.Value, _endPosition.Value);

            _startPosition = null;
            _endPosition = null;
            return startingDirection;
        }
        
        return null;
    }

    public void DrawGizmos()
    {
        _playerSafetyCheck.DrawSafetyPosition();
        
        if (_playerMover.CurrentPlan == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_playerMover.CurrentPlan.TargetLocation, .8f);
        
        Gizmos.DrawLine(_player.transform.position, (Vector2)_player.transform.position + _playerMover.CurrentPlan.MoveDirection * 5f);
    }
}