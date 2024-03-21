using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Savable
{
    public static LevelManager instance { get; private set; }
    public List<Area> areas;
    AreaData areaData;

    [System.Serializable]
    public struct AreaData
    {
        public List<LevelData> level;
    }

    [System.Serializable]
    public struct LevelData
    {
        public string ID;
        public bool isRevealed;
    }

    public override void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);

        if (SaveManager.instance != null)
            SaveManager.instance.RegisterSaveable(this, "LevelManager_0000");
    }

    public override void Start()
    {
        if (SaveManager.instance != null)
            SaveManager.instance.RegisterSaveable(this, "LevelManager_0000");
    }

    public override string SaveData()
    { 
        areaData = new AreaData
        {
            level = new List<LevelData>()
        };

        foreach (Area area in areas)
        {
            foreach (Level level in area.levels)
            {
                LevelData levelData = new LevelData
                {
                    ID = area.name + "_" + level.name,
                    isRevealed = level.isRevealed
                };

                areaData.level.Add(levelData);
            }
        }

        Debug.Log(JsonUtility.ToJson(areaData));
        return JsonUtility.ToJson(areaData);
    }

    public override void LoadDefaultData()
    {
        Debug.Log("Load default level data");

        areaData = new AreaData
        {
            level = new List<LevelData>()
        };

        bool firstTake = true;
        foreach (Area area in areas)
        {
            foreach (Level level in area.levels)
            {
                if (firstTake)
                {
                    firstTake = false;
                    level.isRevealed = true;
                }
                else
                {
                    level.isRevealed = false;
                }
            }
        }

        foreach (LevelData levelData in areaData.level)
        {
            foreach (Area area in areas)
            {
                foreach (Level level in area.levels)
                {
                    if (levelData.ID.Equals(area.name + "_" + level.name))
                    {
                        level.isRevealed = levelData.isRevealed;
                    }
                }
            }
        }

    }

    public override void LoadData(string data, string version)
    {
        Debug.Log("Loading Area data");

        areaData = JsonUtility.FromJson<AreaData>(data);
        foreach (LevelData levelData in areaData.level)
        {
            foreach (Area area in areas)
            {
                foreach (Level level in area.levels)
                {
                    if (levelData.ID.Equals(area.name + "_" + level.name))
                    {
                        level.isRevealed = levelData.isRevealed;
                    }
                }
            }
        }
    }

    public override void OnDestroy()
    {
        SaveManager.instance.UnegisterSaveable(this, "LevelManager_0000");
    }
}
