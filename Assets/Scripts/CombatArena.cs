using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatArena : MonoBehaviour
{
    public List<Door> arenaDoors;
    public List<SpawnPoint> spawnPoints;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        foreach (SpawnPoint spawnPoint in spawnPoints)
        {
            for (int i = 0; i < spawnPoint.spawnPoints.Length; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(spawnPoint.spawnPoints[i] + transform.position, 0.5f);
                GizmosUtils.DrawText(GUI.skin, spawnPoint.enemyPrefab.name, spawnPoint.spawnPoints[i] + transform.position, Color.black, 10, -25);
            }
        }
    }
}


