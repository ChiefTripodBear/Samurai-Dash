﻿using UnityEngine;

public class PlayerMovementManager
{
    private readonly Player _player;
    private Mover _mover;
    private Vector2? _startPosition;
    private Vector2? _endPosition;
    private DestinationSetter _destinationSetter;
    private InputProcessor _inputProcessor;
    private MovementPackage _currentMovementPackage;
    private PlayerSafetyRedirect _playerSafetyRedirect;

    public PlayerMovementManager(Player player)
    {
        _player = player;
        _mover = new Mover(_player, _player.DefaultMovementSpeed);
        _destinationSetter = new DestinationSetter(_player.transform, _mover);
        _inputProcessor = new InputProcessor();
        _playerSafetyRedirect = new PlayerSafetyRedirect(_mover, _player);
    }
    
    public void Tick()
    {
        if (_mover.MovementPackage?.Destination != null)
        {
            _player.PlayerCollider2D.enabled =
                _mover.MovementPackage.Destination.DestinationType == DestinationType.Exit;
        }
        else
        {
            _player.PlayerCollider2D.enabled = true;
        }

        if (_mover.HasDestination)
        {
            _mover.Move();
            _playerSafetyRedirect.Tick();
        }

        if (_playerSafetyRedirect.SafetyRedirectAllowed)
        {
            if (_startPosition.HasValue)
            {
                _mover.Reset();
                _playerSafetyRedirect.SafetyRedirectAllowed = false;
            }
        }    

        var startingDirection = InputDestination();

        if (startingDirection.HasValue == false) return;

        // Allows the player to bailout during a chain... or whenever really.
        // if (_startPosition.HasValue && _endPosition.HasValue) _mover.Reset();
        
        _startPosition = null;
        _endPosition = null;
        if (_mover.IsMoving)
        {
            return;
        }

        _currentMovementPackage = _destinationSetter.GetDestinationFromFirstMove(_player.MoveAmountPerSwipe, startingDirection.Value);

        _mover.SetMovementPackage(_currentMovementPackage);
    }
    
    private Vector2? InputDestination()
    {
        if (_inputProcessor.MouseDown) _startPosition = _inputProcessor.MousePosition();

        if (_inputProcessor.MouseUp && _startPosition.HasValue) _endPosition = _inputProcessor.MousePosition();

        if (_startPosition.HasValue && _endPosition.HasValue)
            return _inputProcessor.GetMoveDirection(_startPosition.Value, _endPosition.Value);

        return null;
    }

    public void DrawGizmos()
    {
        if (_mover.MovementPackage?.Destination != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(_mover.MovementPackage.Destination.TargetLocation, Vector2.one);
        }
    }
}