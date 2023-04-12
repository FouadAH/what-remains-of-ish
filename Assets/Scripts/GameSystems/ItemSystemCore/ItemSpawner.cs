using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject item;
    public Transform targetPosition;

    public void SpawnItem()
    {
        if(targetPosition == null)
        {
            targetPosition = transform;
        }

        item.transform.position = targetPosition.position;
        item.SetActive(true);
    }
}
