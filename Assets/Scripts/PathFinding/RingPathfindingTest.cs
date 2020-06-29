using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class RingPathfindingTest : MonoBehaviour
{
    [SerializeField] private Transform _start;
    [SerializeField] private int _order;
    private Vector2[] _path;
    private RingPosition _targetRing;

    private void Start()
    {
        _targetRing = RingManager.Instance.GetRingPositionFromRingOrder(_order);
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            _targetRing = RingManager.Instance.SwapPositions(_targetRing, _order);
            _path = null;

            while (_targetRing == null || !_targetRing.RingNode.IsWalkable())
            {
                _targetRing = RingManager.Instance.GetRingPositionFromRingOrder(_order);
            }
            var startingRing = RingManager.Instance.FindBestStartingRing(_start.position, _order);
            _path = RingPathFinder.Path(null, startingRing.RingNode, _targetRing.RingNode);
        }
    }

    private void OnDrawGizmosSelected()
    {
        
        if (_path != null)
        {
            for (var i = 0; i < _path.Length; i++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(_path[i], 1f);
            }

            for (var i = 0; i < _path.Length - 1; i++)
            {
                Gizmos.DrawLine(_path[i], _path[i + 1]);
            }
        }

        if (_targetRing != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_targetRing.transform.position, 1f);
        }
    }
}