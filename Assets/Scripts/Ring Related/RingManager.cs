using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RingManager : MonoBehaviour
{
    [SerializeField] private RingPosition _ringPositionPrefab;
    [SerializeField] private float _innerRadius = 5f;
    [SerializeField] private int _ringPositions = 20;
    [SerializeField] private int _numberOfRings = 3;
    [SerializeField] private float _ringPositionCountModifier = 1.5f;
    [SerializeField] private float _ringRadiusModifier = .5f;

    private List<RingPosition> _ringPositionObjects= new List<RingPosition>();
    private Player _player;
    
    private Queue<RingPosition> _positionQueue = new Queue<RingPosition>();
    private List<RingPosition> _orderedRingOrders;
    private RingPosition currentPosition;

    private void Awake()
    {
        _player = FindObjectOfType<Player>();
        
        for (var i = 0; i < _numberOfRings; i++)
        {
            var ringPositionCount = _ringPositions - _ringPositionCountModifier * i;
            var radius = _innerRadius + i * _innerRadius * _ringRadiusModifier;
            
            for (var j = 0; j < ringPositionCount; j++)
            {
                var angle = j * Mathf.PI * 2 / ringPositionCount;

                var ringPositionObject =
                    Instantiate(_ringPositionPrefab, _player.transform, true);

                ringPositionObject.name = $"{j + 1} Ring Position - {i + 1}";
                ringPositionObject.Initialize(angle, radius, i + 1);

                _ringPositionObjects.Add(ringPositionObject);
            }
        }
        
        _orderedRingOrders = _ringPositionObjects.Where(t => t.RingOrder == 1).ToList();

        currentPosition = _orderedRingOrders[Random.Range(0, _orderedRingOrders.Count)];

        _positionQueue.Enqueue(currentPosition);
        
        PopulatePositionQueue();
    }

    private void PopulatePositionQueue()
    {
        while (true)
        {
            var oppositePosition = GetBestPositionFromOppositePoint(currentPosition.OppositePoint);

            if (oppositePosition == null) return;

            currentPosition = oppositePosition;
            
            _positionQueue.Enqueue(currentPosition);
        }
    }

    private RingPosition GetBestPositionFromOppositePoint(Vector2 currentPositionOppositePoint)
    {
        return _ringPositionObjects
            .Where(t => !t.Claimed && !_positionQueue.Contains(t))
            .OrderBy(t => Vector2.Distance(t.transform.position, currentPositionOppositePoint))
            .FirstOrDefault();
    }

    public RingPosition GetNextPosition(RingPosition previousPosition)
    { 
        if(previousPosition != null)
            _positionQueue.Enqueue(previousPosition);

        var nextPosition = _positionQueue.Count > 0 &&  _positionQueue.Peek() != previousPosition ? _positionQueue.Dequeue() : null;
        
        return nextPosition;
    }
    
    // private void OnDrawGizmos()
    // {
    //     if (_positionQueue == null || _positionQueue.Count <= 0) return;
    //
    //     var positionList = _positionQueue.ToList();
    //
    //     for (var i = 1; i < positionList.Count; i++)
    //     {
    //         Gizmos.DrawLine(positionList[i].transform.position, positionList[i - 1].transform.position);
    //     }
    // }
}

