using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newPrefabReference", menuName = "New Prefab Reference", order = 1)]
public class PrefabReferenceSO : ScriptableObject
{
    [SerializeField] private GameObject prefab;
    public GameObject GetPrefab()
    {
        return prefab;
    }
}
