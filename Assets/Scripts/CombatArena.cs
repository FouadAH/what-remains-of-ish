using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatArena : MonoBehaviour
{
    public List<Door> arenaDoors;
    public List<CombatRound> combatRounds;

    bool hasBeenActivated;
    bool allRoundsOver;
    int currentCombatRound = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasBeenActivated || allRoundsOver)
            return;

        if (collision.GetComponent<Player>())
        {
            hasBeenActivated = true;
            foreach (Door door in arenaDoors)
            {
                door.SetState(false);
            }

            combatRounds[currentCombatRound].SpawnEnemies();
        }
    }

    public void OnComabatRoundOver()
    {
        allRoundsOver = true;

        foreach (CombatRound combatRound in combatRounds)
        {
            if (!combatRound.allEnemiesDead)
                allRoundsOver = false;
        }

        if (allRoundsOver)
        {
            foreach (Door door in arenaDoors)
            {
                door.SetState(true);
            }
        }
        else
        {
            currentCombatRound++;
            combatRounds[currentCombatRound].SpawnEnemies();
        }
    }

    private void OnDrawGizmos()
    {
        foreach (CombatRound spawnPoint in combatRounds)
        {
            for (int i = 0; i < spawnPoint.spawnPoints.Length; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(spawnPoint.spawnPoints[i].position + transform.position, 0.5f);
                GizmosUtils.DrawText(GUI.skin, spawnPoint.enemyPrefab.name, spawnPoint.spawnPoints[i].position + transform.position, Color.black, 10, -25);
            }
        }
    }
}


