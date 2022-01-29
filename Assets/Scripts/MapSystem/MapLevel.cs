using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapLevel : Savable
{
    public Level level;
    [HideInInspector] public Image levelImage;
    public MapRoomData mapRoomData;

    public struct MapRoomData
    {
        public bool isRevealed;
    }

    public override void Start()
    {
        base.Start();
        levelImage = GetComponent<Image>();
        mapRoomData.isRevealed = level.isRevealed;
        levelImage.enabled = level.isRevealed;
    }

    public override string SaveData()
    {
        mapRoomData.isRevealed = level.isRevealed;
        return JsonUtility.ToJson(mapRoomData);
    }

    public override void LoadDefaultData()
    {
        mapRoomData.isRevealed = level.isRevealed;
    }

    public override void LoadData(string data, string version)
    {
        mapRoomData = JsonUtility.FromJson<MapRoomData>(data);
        level.isRevealed = mapRoomData.isRevealed;
        Debug.Log("Loading MapRoom data: " + data);
    }
}
