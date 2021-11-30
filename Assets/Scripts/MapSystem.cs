using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSystem : MonoBehaviour
{
    public List<MapArea> areas;
    public RectTransform UI_Element;
    public Canvas mapCanvas;

    RectTransform CanvasRect;
    GameObject WorldObject;
    Vector3 initialPos;

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

    private void Start()
    {
        CanvasRect = mapCanvas.GetComponent<RectTransform>();
        WorldObject = GameManager.instance.player;
        initialPos = GameManager.instance.initialPlayerPosition;
    }

    private void Update()
    {
        UI_Element.anchoredPosition = WorldObject.transform.position - initialPos;
    }


}
