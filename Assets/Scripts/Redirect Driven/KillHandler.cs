using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillHandler : MonoBehaviour
{
    private readonly Queue<EnemyAngle> _killQueue = new Queue<EnemyAngle>();

    private void Awake()
    {
        EnemyAngle.OnKillPointReached += AddToKillQueue;
        RedirectPlayer.OnDestinationReached += Kill;
    }

    private void OnDestroy()
    {
        EnemyAngle.OnKillPointReached -= AddToKillQueue;
        RedirectPlayer.OnDestinationReached -= Kill;
    }

    private void AddToKillQueue(EnemyAngle enemyAngle)
    {
        if(!_killQueue.Contains(enemyAngle))
            _killQueue.Enqueue(enemyAngle);
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