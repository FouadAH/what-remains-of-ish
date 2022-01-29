using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSystem : MonoBehaviour
{
    public List<MapArea> areas;
    public RectTransform UI_Element;
    public Canvas mapCanvas;
    public PlayerConfig initialPlayerData;
    
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
                    Debug.Log("Revealing Room: " + mapLevel.level.name);
                    mapLevel.levelImage.enabled = true;
                }
            }
        }
    }

    private void Start()
    {
        WorldObject = GameManager.instance.player;
        initialPos = GameManager.instance.initalPlayerData.initialPlayerPosition;
    }

    private void Update()
    {
        UI_Element.anchoredPosition = WorldObject.transform.position - initialPos;
    }

    //private void OnValidate()
    //{
    //    WorldObject = FindObjectOfType<Player>().gameObject;
    //    initialPos = new Vector2(660, 80); 
    //    UI_Element.anchoredPosition = WorldObject.transform.position - initialPos;

    //}


}
