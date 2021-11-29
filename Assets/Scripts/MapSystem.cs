using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSystem : MonoBehaviour
{
    public List<MapArea> areas;

    public void OnNewLevelLoad()
    {
        foreach (MapArea mapArea in areas)
        {
            Debug.Log(mapArea.area.areaName);
            Debug.Log("------------------");

            foreach (MapLevel mapLevel in mapArea.mapLevels)
            {
                if (mapLevel.level.isRevealed)
                {
                    Debug.Log(mapLevel.level.name);
                    mapLevel.levelImage.enabled = true;
                }
            }
        }
    }

}
