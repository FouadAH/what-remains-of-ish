using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    public Area area;
    public List<MapLevel> mapLevels;

    public void AddLevelsToAreaList()
    {
        mapLevels.Clear();
        MapLevel[] mapLevelsArray = GetComponentsInChildren<MapLevel>();
        foreach(MapLevel mapLevel in mapLevelsArray)
        {
            mapLevels.Add(mapLevel);
        }
    }

    public MapLevel[] GetAreaLevels()
    {
        return GetComponentsInChildren<MapLevel>();
    }

}
