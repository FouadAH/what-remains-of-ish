using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public GameEvent SpawnerEnemiesDeadEvent;

    List<Entity> enemies = new List<Entity>();
    public bool allEnemiesDead;

    public void SpawnEnemies()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            GameObject enemyGO = Instantiate(enemyPrefab, spawnPoint);
            Entity entity = enemyGO.GetComponentInChildren<Entity>();
            enemies.Add(entity);
            entity.OnDeath += OnEnemyDeath;
        }
    }

    bool enemiesDeadTemp;
    public void OnEnemyDeath()
    {
        enemiesDeadTemp = true;

        foreach (Entity enemy in enemies)
        {
            if (!enemy.isDead)
                enemiesDeadTemp = false;
        }

        if (enemiesDeadTemp)
        {
            allEnemiesDead = true;
            SpawnerEnemiesDeadEvent.Raise();
        }
    }
}
