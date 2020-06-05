using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillHandler : MonoBehaviour
{
    private readonly Queue<EnemyKillHandler> _killQueue = new Queue<EnemyKillHandler>();

    private void Awake()
    {
        EnemyKillHandler.OnKillPointReached += AddToKillQueue;
        PlayerMover.OnDestinationReached += Kill;
    }

    private void OnDestroy()
    {
        EnemyKillHandler.OnKillPointReached -= AddToKillQueue;
        PlayerMover.OnDestinationReached -= Kill;
    }

    private void AddToKillQueue(EnemyKillHandler enemyKillHandler)
    {
        if (!_killQueue.Contains(enemyKillHandler))
        {
            enemyKillHandler.GetComponent<PathfindingUnit>().CanMove = false;
            _killQueue.Enqueue(enemyKillHandler);
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