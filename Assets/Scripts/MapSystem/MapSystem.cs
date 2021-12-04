using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSystem : MonoBehaviour
{
    public List<MapArea> areas;
    public RectTransform UI_Element;
    public Canvas mapCanvas;

    GameObject WorldObject;
    Vector3 initialPos;

    public void OnNewLevelLoad()
    {
        foreach (MapArea mapArea in areas)
        {
            foreach (MapLevel mapLevel in mapArea.mapLevels)
            {
                if (mapLevel.level.isRevealed)
                {
                    mapLevel.levelImage.enabled = true;
                }
            }
        }
    }

    private void Start()
    {
        WorldObject = GameManager.instance.player;
        initialPos = GameManager.instance.playerStartPosition;
    }

    private void Update()
    {
        UI_Element.anchoredPosition = WorldObject.transform.position - initialPos;
    }


}
