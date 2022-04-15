using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatArena : Savable
{
    [System.Serializable]
    public struct Data
    {
        public bool isDone;
    }

    [SerializeField]
    private Data combatArenaData;

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

            combatRounds[currentCombatRound].StartRound();
            AudioManager.instance.SetIntensity(75);
        }
    }

    public void OnCombatRoundOver()
    {
        allRoundsOver = true;

        foreach (CombatRound combatRound in combatRounds)
        {
            if (!combatRound.IsAllEnemiesDead())
                allRoundsOver = false;
        }

        if (allRoundsOver)
        {
            combatArenaData.isDone = true;
            AudioManager.instance.SetIntensity(0);

            foreach (Door door in arenaDoors)
            {
                door.SetState(true);
            }
        }
        else
        {
            currentCombatRound++;
            Debug.Log("Current: " + currentCombatRound);
            combatRounds[currentCombatRound].StartRound();
        }
    }

    private void OnDrawGizmos()
    {
        foreach (CombatRound spawnPoint in combatRounds)
        {
            foreach (Spawner spawner in spawnPoint.spawners)
            {
                for (int i = 0; i < spawner.spawnPoints.Length; i++)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(spawner.spawnPoints[i].position + transform.position, 0.5f);
                    GizmosUtils.DrawText(GUI.skin, spawner.enemyPrefab.name, spawner.spawnPoints[i].position + transform.position, Color.black, 10, -25);
                }
            }
        }
    }

    public override string SaveData()
    {
        return JsonUtility.ToJson(combatArenaData);
    }

    public override void LoadDefaultData()
    {
        combatArenaData.isDone = false;
        foreach (Door door in arenaDoors)
        {
            door.SetState(true);
        }

    }

    public override void LoadData(string data, string version)
    {
        combatArenaData = JsonUtility.FromJson<Data>(data);
        Debug.Log("loading puzzle state: " + combatArenaData.isDone);

        foreach (Door door in arenaDoors)
        {
            door.SetState(true);
        }
        if (combatArenaData.isDone)
        {
            allRoundsOver = true;
        }
        else
        {
            allRoundsOver = false;
        }
    }
}


