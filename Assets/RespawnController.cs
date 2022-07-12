using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnController : MonoBehaviour
{
    public PrefabReferenceSO prefabReference;
    Transform spawnTransform;

    void Start()
    {
        spawnTransform = transform;
    }

    public void OnRespawnEvent()
    {
        Instantiate(prefabReference.GetPrefab(), spawnTransform.position, spawnTransform.rotation);
        Destroy(gameObject);
    }

}
