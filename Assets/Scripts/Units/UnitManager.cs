using System.Collections.Generic;
using UnityEngine;

public abstract class UnitManager<T> : MonoBehaviour where T : Unit
{
    [SerializeField] private int _ringOrder;
    
    protected List<T> Units = new List<T>();

    private Player _player;

    private static UnitManager<T> _instance;
    public static UnitManager<T> Instance => _instance;

    private void Awake()
    {
        _player = FindObjectOfType<Player>();
        _instance = this;
    }

    public void RegisterUnit(T unit)
    {
        Units.Add(unit);
        GameUnitManager.RegisterUnit(unit);
        UnitChainEvaluator.Instance.RegisterKillableUnit(unit);
    }

    public void RemoveUnit(T unit)
    {
        Units.Remove(unit);
        GameUnitManager.RemoveUnit(unit);
    }
    
    private void SetUnitPositions()
    {
        if (!RingManager.Instance.HasPositions(_ringOrder)) return;
        
        foreach (var unit in Units)
        {
            if(unit.UnitPathFinder.CurrentRingPosition != null) continue;
            
            unit.UnitPathFinder.SetRingPosition(RingManager.Instance.GetRingPositionFromRingOrder(_ringOrder));
        }
    }

    protected virtual void Update()
    {
        SetUnitPositions();
    }

    public RingPosition GetNextAvailableRingPosition(RingPosition ringPosition)
    {
        return RingManager.Instance.SwapPositions(ringPosition, _ringOrder);
    }
}