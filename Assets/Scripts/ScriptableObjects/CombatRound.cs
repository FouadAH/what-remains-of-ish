using System.Collections.Generic;
using UnityEngine;

public class CombatRound : MonoBehaviour
{
    public GameEvent AllEnemiesDeadEvent;
    public List<Spawner> spawners = new List<Spawner>();

    public bool IsAllEnemiesDead()
    {
        bool allDead = true;
        foreach (Spawner spawner in spawners)
        {
            if (!spawner.allEnemiesDead)
            {
                allDead = false;
            }
        }
        return allDead;
    }

    public void OnSpawnerEnemiesDead()
    {
        Debug.Log("OnSpawnerEnemiesDead");
        if (IsAllEnemiesDead())
        {
            AllEnemiesDeadEvent.Raise();
        }
    }

    public void StartRound()
    {
        foreach (Spawner spawner in spawners)
        {
            spawner.SpawnEnemies();
        }
    }
}