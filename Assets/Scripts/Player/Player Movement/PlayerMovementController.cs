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
    }

    private IUnit _currentUnit;

    public void Tick()
    {
        if (!_playerMover.IsMoving) _playerDashMonitor.RefundChargesWhileNotMoving();
        
        if (_playerMover.DEBUGCurrentPlan?.TargetUnit != null)
        {
            if (_currentUnit != null) _currentUnit.Transform.GetComponent<SpriteRenderer>().color = Color.white;
            _currentUnit = _playerMover.DEBUGCurrentPlan.TargetUnit;
            _currentUnit.Transform.GetComponent<SpriteRenderer>().color = Color.green;
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
        if (_playerMover.DEBUGCurrentPlan == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_playerMover.DEBUGCurrentPlan.TargetLocation, .8f);
        
        Gizmos.DrawLine(_player.transform.position, (Vector2)_player.transform.position + _playerMover.DEBUGCurrentPlan.MoveDirection * 5f);
    }
}