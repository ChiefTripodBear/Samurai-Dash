using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillHandler : MonoBehaviour
{
    private readonly Queue<UnitKillHandler> _killQueue = new Queue<UnitKillHandler>();

    private void Awake()
    {
        UnitKillHandler.OnKillPointReached += AddToKillQueue;
        PlayerMover.OnDestinationReached += Kill;
    }

    private void OnDestroy()
    {
        UnitKillHandler.OnKillPointReached -= AddToKillQueue;
        PlayerMover.OnDestinationReached -= Kill;
    }

    private void AddToKillQueue(UnitKillHandler unitKillHandler)
    {
        if (!_killQueue.Contains(unitKillHandler))
        {
            var pathFinder = unitKillHandler.GetComponent<IUnitPathFinder>();
            if (pathFinder != null)
                pathFinder.CanMoveThroughPath = false;
            
            _killQueue.Enqueue(unitKillHandler);
        }
    }

    private void Kill()
    {
        if (_killQueue.Count <= 0) return;
        
        StartCoroutine(KillWithDelay());
    }

    private IEnumerator KillWithDelay()
    {
        var killCount = _killQueue.Count;
        
        for (var i = 0; i < killCount; i++)
        {
            if(_killQueue == null || _killQueue.Count <= 0) yield break;

            if(_killQueue.Peek() == null) continue;
            
            var enemy = _killQueue.Dequeue();
            enemy.Kill();
            yield return new WaitForSeconds(.5f);
        }
    }
}