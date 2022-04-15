using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveSpawner : MonoBehaviour
{
    public GameObject shockwavePrefab;

    public void SpawnShockwave(int direction)
    {
        GameObject showckwave = Instantiate(shockwavePrefab, new Vector3(transform.position.x , transform.position.y, 1), Quaternion.identity);
        showckwave.transform.localScale = new Vector3(direction, 1, 1);
    }
}
