﻿using System.Collections.Generic;
using UnityEngine;

public abstract class UnitManager : MonoBehaviour
{
    [SerializeField] private int _ringOrder;
    
    protected List<IUnitEnemy> Units = new List<IUnitEnemy>();

    public void RegisterUnit(IUnitEnemy unit)
    {
        if (Units.Contains(unit)) return;

        Units.Add(unit);
        GameUnitManager.RegisterUnit(unit);
        UnitChainEvaluator.Instance.AddUnit(unit);
    }

    public void RemoveUnit(IUnitEnemy unit)
    {
        Units.Remove(unit);
        GameUnitManager.RemoveUnit(unit);
    }

    public RingPosition GetNextAvailableRingPosition(IWaypoint ringPosition)
    {
        return RingManager.Instance.SwapPositions(ringPosition as RingPosition, _ringOrder);
    }

    public void TurnInRingPosition(RingPosition ringPosition)
    {
        RingManager.Instance.TurnInPosition(ringPosition, _ringOrder);
    }
}