using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSystem : MonoBehaviour
{
    public List<Area> areas;

    void Start()
    {
        foreach (Area area in areas)
        {
            foreach (Level level in area.levels)
            {
                Debug.Log(level.scene);
            }
        }
    }

}
