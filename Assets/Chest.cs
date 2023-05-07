using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public ItemSpawner itemSpawner;

    public void OnOpenChest()
    {
        itemSpawner.SpawnItem();
    }
}
