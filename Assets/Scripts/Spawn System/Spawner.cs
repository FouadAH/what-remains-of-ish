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
    public bool showSpawnPoints = true;
    public FacingDirection spawnFacingDirection = FacingDirection.Left;
    public ParticleSystem spawnEffect;

    public void SpawnEnemies()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(0.65f);

        foreach (Transform spawnPoint in spawnPoints)
        {
            Instantiate(spawnEffect, spawnPoint.transform.position, Quaternion.identity, spawnPoint);
        }

        yield return new WaitForSeconds(0.15f);

        foreach (Transform spawnPoint in spawnPoints)
        {
            GameObject enemyGO = Instantiate(enemyPrefab, spawnPoint.transform.position, Quaternion.identity, spawnPoint);

            Entity entity = enemyGO.GetComponentInChildren<Entity>();

            if (!entity.IsFacingPlayer()) 
            {
                entity.Flip(); 
            }

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

    private void OnDrawGizmos()
    {
        if (!showSpawnPoints)
        {
            return;
        }

        Gizmos.color = Color.red;
        foreach (var item in spawnPoints)
        {
            Gizmos.DrawSphere(item.transform.position, 0.3f);
        }
    }
}

public enum FacingDirection
{
    Left,
    Right,
}
