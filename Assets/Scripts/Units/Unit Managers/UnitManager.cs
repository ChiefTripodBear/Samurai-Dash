using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitManager : MonoBehaviour
{
    [SerializeField] private int _ringOrder;
    
    protected List<IUnitEnemy> Units = new List<IUnitEnemy>();

    protected Player Player { get; private set; }

    private void Awake()
    {
        Player = FindObjectOfType<Player>();
    }

    public void RegisterUnit(IUnitEnemy unit)
    {
        if (Units.Contains(unit)) return;

        Units.Add(unit);
        GameUnitManager.RegisterUnit(unit);
        UnitChainEvaluator.Instance.AddUnit(unit);
        OnRegisterUnit(unit);
    }

    protected abstract void OnRegisterUnit(IUnitEnemy unitEnemy);
    protected abstract void OnRemoveUnit(IUnitEnemy unitEnemy);
    
    public void RemoveUnit(IUnitEnemy unit)
    {
        Units.Remove(unit);
        GameUnitManager.RemoveUnit(unit);
        OnRemoveUnit(unit);
    }

    public RingPosition GetNextAvailableRingPosition(IWaypoint ringPosition)
    {
        return RingManager.Instance.SwapPositions(ringPosition as RingPosition, _ringOrder);
    }

    public void TurnInRingPosition(RingPosition ringPosition)
    {
        RingManager.Instance.TurnInPosition(ringPosition, _ringOrder);
    }

    public RingPosition FindBestStartingNode(Vector2 startingPosition)
    {
        return RingManager.Instance.FindBestStartingRing(startingPosition, _ringOrder);
    }
}