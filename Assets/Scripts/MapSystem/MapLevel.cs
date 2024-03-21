using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapLevel : MonoBehaviour
{
    public Level level;
    public Image levelImage;
    public MapRoomData mapRoomData;

    public struct MapRoomData
    {
        public bool isRevealed;
    }

    public void Start()
    {
        levelImage = GetComponent<Image>();
        mapRoomData.isRevealed = level.isRevealed;
        levelImage.enabled = level.isRevealed;
    }

}
