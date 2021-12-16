using System.Collections.Generic;
using UnityEngine;

public class CombatRound : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public GameEvent AllEnemiesDeadEvent;

    List<Entity> enemies = new List<Entity>();
    public bool allEnemiesDead;

    public void SpawnEnemies()
    {
        foreach(Transform spawnPoint in spawnPoints)
        {
            GameObject enemyGO = Instantiate(enemyPrefab, spawnPoint);
            Entity entity = enemyGO.GetComponentInChildren<Entity>();
            enemies.Add(entity);
            entity.OnDeath += OnEnemyDeath;

        }
    }

    public void OnEnemyDeath()
    {
        allEnemiesDead = true;

        foreach (Entity enemy in enemies)
        {
            if (!enemy.isDead)
                allEnemiesDead = false;
        }

        if (allEnemiesDead)
        {
            AllEnemiesDeadEvent.Raise();
        }
    }
}