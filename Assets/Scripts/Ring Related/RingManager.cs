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
    
    private readonly Dictionary<int, List<RingPosition>> _ringOrders = new Dictionary<int, List<RingPosition>>();
    private readonly Dictionary<int, Queue<RingPosition>> _ringPositionQueues = new Dictionary<int, Queue<RingPosition>>();
    
    private static RingManager _instance;

    public static RingManager Instance => _instance;

    private Player _player;

    private void Awake()
    {
        if (_instance == null) _instance = this;

        _player = FindObjectOfType<Player>();
        
        for (var i = 0; i < _numberOfRings; i++)
        {
            var ringPositionCount = _ringPositions - _ringPositionCountModifier * i;
            var radius = _innerRadius + i * _innerRadius * _ringRadiusModifier;
            
            var positionsOnRing = new List<RingPosition>();
            for (var j = 0; j < ringPositionCount; j++)
            {
                var angle = j * Mathf.PI * 2 / ringPositionCount;

                var ringPositionObject =
                    Instantiate(_ringPositionPrefab, _player.transform, true);

                ringPositionObject.name = $"{j + 1} Ring Position - {i}";
                ringPositionObject.Initialize(angle, radius, i);

                positionsOnRing.Add(ringPositionObject);
            }

            _ringOrders.Add(i, positionsOnRing);
            _ringPositionQueues[i] = PopulateQueue(i, _ringOrders[i]);
        }
    }

    private Queue<RingPosition> PopulateQueue(int ringOrder, List<RingPosition> ringPositions)
    {
        var positionQueue = new Queue<RingPosition>();
        var currentRingPosition = ringPositions[Random.Range(0, ringPositions.Count)];

        while (true)
        {
            var oppositePosition = GetBestPositionFromOppositePoint(ringOrder, currentRingPosition.OppositePoint, positionQueue);

            if (oppositePosition == null) return positionQueue;

            currentRingPosition = oppositePosition;

            positionQueue.Enqueue(currentRingPosition);
        }
    }
    
    private RingPosition GetBestPositionFromOppositePoint(int ringOrder, Vector2 currentPositionOppositePoint, Queue<RingPosition> ringOrderQueue)
    {
        return _ringOrders[ringOrder]
            .Where(t => !t.Claimed && !ringOrderQueue.Contains(t))
            .OrderBy(t => Vector2.Distance(t.transform.position, currentPositionOppositePoint))
            .FirstOrDefault();
    }

    public RingPosition SwapPositions(RingPosition previousPosition, int order)
    {
        var ringQueue = _ringPositionQueues[order];

        var nextAvailable = ringQueue.Count > 0 ? ringQueue.Dequeue() : null;

        if (previousPosition != null)
            ringQueue.Enqueue(previousPosition);

        return nextAvailable;
    }
    
    public RingPosition GetRingPositionFromRingOrder(int ringOrder)
    {
        var position = _ringPositionQueues[ringOrder].Peek() != null ? _ringPositionQueues[ringOrder].Dequeue() : null;
        return position;
    }

    public bool HasPositions(int ringOrder)
    {
        return _ringPositionQueues[ringOrder].Count > 0;
    }
}