using System.Collections.Generic;
using UnityEngine;

public abstract class UnitManager<T> : MonoBehaviour where T : IUnitEnemy
{
    [SerializeField] private int _ringOrder;
    
    protected List<T> Units = new List<T>();
    
    private static UnitManager<T> _instance;
    public static UnitManager<T> Instance => _instance;

    private void Awake()
    {
        _instance = this;
    }

    public void RegisterUnit(T unit)
    {
        if (Units.Contains(unit)) return;

        Units.Add(unit);
        GameUnitManager.RegisterUnit(unit);
        UnitChainEvaluator.Instance.AddUnit(unit);
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
            if(unit.UnitPathFinder.CurrentRingPosition != null || unit.Transform.gameObject.activeInHierarchy == false) continue;
            
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

    public void TurnInRingPosition(RingPosition ringPosition)
    {
        RingManager.Instance.TurnInPosition(ringPosition, _ringOrder);
    }
}