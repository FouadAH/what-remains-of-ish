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
    public ScrollRect scrollRect;
    public float scrollSpeed = 2f;

    GameObject WorldObject;
    Vector3 initialPos;
    Player_Input playerInput;

    private void Start()
    {
        WorldObject = GameManager.instance.player;
        initialPos = GameManager.instance.initalPlayerData.initialPlayerPosition;
        playerInput  = GameManager.instance.player.GetComponent<Player_Input>();
        RevealRooms();
    }

    private void Update()
    {
        playerIcon.anchoredPosition = WorldObject.transform.position - initialPos;
        mapContent.anchoredPosition += playerInput.mapRightStickInput * scrollSpeed;
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

    public void OpenMap()
    {
        Debug.Log("Opened Map");
        SnapTo(playerIcon);
    }

    public void SnapTo(RectTransform target)
    {
        Canvas.ForceUpdateCanvases();

        mapContent.anchoredPosition =
                (Vector2)scrollRect.transform.InverseTransformPoint(mapContent.position)
                - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);
    }
}
