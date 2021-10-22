using UnityEngine;

[CreateAssetMenu(fileName = "SpawnPoint", menuName = "SpawnPoints/New Spawn Point", order = 1)]
public class SpawnPoint : ScriptableObject
{
    public GameObject enemyPrefab;
    public Vector3[] spawnPoints;
}