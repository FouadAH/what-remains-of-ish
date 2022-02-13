using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSystem : MonoBehaviour
{
    public List<MapArea> areas;
    public RectTransform playerIcon;
    public PlayerConfig initialPlayerData;
    public RectTransform mapContent;

    GameObject WorldObject;
    Vector3 initialPos;

    private void Start()
    {
        WorldObject = GameManager.instance.player;
        initialPos = GameManager.instance.initalPlayerData.initialPlayerPosition;
        RevealRooms();
    }

    private void Update()
    {
        playerIcon.anchoredPosition = WorldObject.transform.position - initialPos;
    }

    public void OnNewLevelLoad()
    {
        RevealRooms();
    }

    public void RevealRooms()
    {
        foreach (MapArea mapArea in areas)
        {
            foreach (MapLevel mapLevel in mapArea.mapLevels)
            {
                if (mapLevel.level.isRevealed)
                {
                    Debug.Log("Revealing Room: " + mapLevel.level.name);

                    if (mapLevel.levelImage == null)
                        mapLevel.levelImage = mapLevel.GetComponent<Image>();

                    mapLevel.levelImage.enabled = true;
                }
            }
        }
    }

    private void OnEnable()
    {
        mapContent.anchoredPosition = playerIcon.anchoredPosition;
    }
}
