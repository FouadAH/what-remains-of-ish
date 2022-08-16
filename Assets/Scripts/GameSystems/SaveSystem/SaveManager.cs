using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System;
using Newtonsoft.Json;

public class SaveManager : MonoBehaviour
{
    public List<SaveFileSO> saveFiles;

    public GameEvent loadSaveFileEvent;
    public GameEvent onSavedData;
    public GameEvent newGameEvent;

#if UNITY_EDITOR
    public SaveFileSO testSaveFile;
#endif

    public SaveFileSO currentSaveFile;
    string savePath;  //TO:DO : get this from config file
    string testSavePath;  //TO:DO : get this from config file
    public PlayerConfig initialPlayerData;
    public static SaveManager instance;
    private Dictionary<string, ISaveable> m_RegisteredSaveables = null;

    public PlayerDataSO playerDataSO;

    SceneData sceneDataCache;
    EnemyData enemyDataCache;
    
    private void Awake()
    {
        savePath = Application.persistentDataPath + "/SaveFiles";
        testSavePath = Application.persistentDataPath + "/TestSaveFiles";

        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        if (!Directory.Exists(testSavePath))
        {
            Directory.CreateDirectory(testSavePath);
        }


        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        InitSaveManager();
    }

    public void InitSaveManager()
    {
#if UNITY_EDITOR
        SetupTestingSaveFile();
#endif

        saveFiles.Clear();
        //Searching through disk for the save files
        foreach (string filePath in Directory.GetFiles(savePath))
        {
            //Should Load metadata file instead
            GameData gameData = LoadDataFromFile(filePath);

            SaveFileSO saveFileSO = ScriptableObject.CreateInstance<SaveFileSO>();
            saveFileSO.fileName = Path.GetFileName(filePath);
            saveFileSO.path = filePath;
            saveFileSO.creationTime = Directory.GetCreationTime(savePath).ToShortDateString();
            saveFileSO.gameData = gameData;
            saveFileSO.slotIndex = int.Parse(Path.GetFileName(filePath).Split(".")[0].Split("_")[1]);

            saveFiles.Add(saveFileSO);
        }
    }

#if UNITY_EDITOR

    private void SetupTestingSaveFile()
    {
        string testFileName = "TestSaveFile.json";
        string path = testSavePath + "/" + testFileName;

        if (!File.Exists(path))
        {
            StreamWriter sw = File.CreateText(path);
            sw.Close();
        }

        testSaveFile = ScriptableObject.CreateInstance<SaveFileSO>();
        testSaveFile.fileName = testFileName;
        testSaveFile.path = path;
        testSaveFile.creationTime = Directory.GetCreationTime(path).ToShortDateString();

        GameData gameDataTest = DefaultGameData();

        testSaveFile.gameData = gameDataTest;

        currentSaveFile = testSaveFile;
        sceneDataCache = currentSaveFile.gameData.scene_data;
        enemyDataCache = currentSaveFile.gameData.enemy_data;
    }
#endif

    public void RegisterSaveable(ISaveable saveable, string ID)
    {
        if (m_RegisteredSaveables == null)
        {
            m_RegisteredSaveables = new Dictionary<string, ISaveable>();
        }

        if(!m_RegisteredSaveables.ContainsKey(ID))
            m_RegisteredSaveables.Add(ID, saveable);
    }

    public void UnegisterSaveable(ISaveable saveable, string ID)
    {
        if (m_RegisteredSaveables == null)
        {
            m_RegisteredSaveables = new Dictionary<string, ISaveable>();
        }

        if (m_RegisteredSaveables.ContainsKey(ID))
            m_RegisteredSaveables.Remove(ID);
    }

    //Saving functions

    /// <summary>
    /// Loads data from a file.
    /// </summary>
    private void DataFileToCache()
    {
        StreamReader reader = new StreamReader(currentSaveFile.path);

        string data = reader.ReadToEnd();

        reader.Close();

        currentSaveFile.gameData = JsonConvert.DeserializeObject<GameData>(data);
        sceneDataCache = currentSaveFile.gameData.scene_data;
        enemyDataCache = currentSaveFile.gameData.enemy_data;
        currentSaveFile.gameData.OnLoad();
    }

    /// <summary>
    /// Loads data for all the saveables.
    /// </summary>
    public void DataCacheToSaveables()
    {
        if (m_RegisteredSaveables == null)
        {
            m_RegisteredSaveables = new Dictionary<string, ISaveable>();
        }

        if (currentSaveFile.gameData.scene_data.data_entries == null)
        {
            Debug.LogError("SaveManager : Trying to move data to saveables while there is no data in the save cache. Write to cache first.");
            return;
        }

        foreach (KeyValuePair<string, ISaveable> saveablePair in m_RegisteredSaveables)
        {
            if (saveablePair.Value is Entity)
            {
                if (enemyDataCache.data_entries.ContainsKey(saveablePair.Key))
                {
                    saveablePair.Value.LoadData(enemyDataCache.data_entries[saveablePair.Key], "0");
                }
                else
                {
                    Debug.Log("SaveManager : Data from key " + saveablePair.Key.ToString() + " not found. Setting default values.");
                    saveablePair.Value.LoadDefaultData();
                }
            }
            else
            {
                if (sceneDataCache.data_entries.ContainsKey(saveablePair.Key))
                {
                    saveablePair.Value.LoadData(sceneDataCache.data_entries[saveablePair.Key], "0");
                }
                else
                {
                    Debug.Log("SaveManager : Data from key " + saveablePair.Key.ToString() + " not found. Setting default values.");
                    saveablePair.Value.LoadDefaultData();
                }
            }
        }

        //OnFinishedLoadData
    }


    public void WriteDataToDisk()
    {
        currentSaveFile.gameData.scene_data = sceneDataCache;
        currentSaveFile.gameData.enemy_data = enemyDataCache;
        Debug.Log("scene data: " + sceneDataCache.data_entries.Count);

        currentSaveFile.gameData.OnWrite();

        string data = JsonConvert.SerializeObject(currentSaveFile.gameData);
        Debug.Log("Data: " + data);
        string path = currentSaveFile.path;
        if (!File.Exists(path))
        {
            StreamWriter sw = File.CreateText(path);
            sw.Close();
        }
        File.WriteAllText(path, data);
        Debug.Log("Saved Game Data to: " + path);
    }

    public void SavePlayerData()
    {
        PlayerData playerData = GetPlayerData();
        currentSaveFile.gameData.player_data = playerData;
        WriteDataToDisk();
    }

    public void SaveSceneData()
    {
        if (sceneDataCache.data_entries == null)
        {
            sceneDataCache.data_entries = new Dictionary<string, string>();
        }

        if (enemyDataCache.data_entries == null)
        {
            enemyDataCache.data_entries = new Dictionary<string, string>();
        }
        if (m_RegisteredSaveables == null)
        {
            m_RegisteredSaveables = new Dictionary<string, ISaveable>();
        }

        foreach (KeyValuePair<string, ISaveable> saveablePair in m_RegisteredSaveables)
        {
            if (saveablePair.Value is Entity)
            {
                enemyDataCache.data_entries[saveablePair.Key.ToString()] = saveablePair.Value.SaveData();
            }
            else
            {
                sceneDataCache.data_entries[saveablePair.Key.ToString()] = saveablePair.Value.SaveData();
            }
        }

        WriteDataToDisk();
    }

    public void RestPointSave()
    {
        PlayerData playerData = GetPlayerData();
        currentSaveFile.gameData.player_data = playerData;

        if (sceneDataCache.data_entries == null)
        {
            sceneDataCache.data_entries = new Dictionary<string, string>();
        }
        if (m_RegisteredSaveables == null)
        {
            m_RegisteredSaveables = new Dictionary<string, ISaveable>();
        }

        foreach (KeyValuePair<string, ISaveable> saveablePair in m_RegisteredSaveables)
        {
            sceneDataCache.data_entries[saveablePair.Key.ToString()] = saveablePair.Value.SaveData();
        }

        enemyDataCache.data_entries = new Dictionary<string, string>();

        WriteDataToDisk();
        DataCacheToSaveables();
    }

    public void SaveGame()
    {
        onSavedData.Raise();
        PlayerData playerData = GetPlayerData();
        SavablesToCache();
        currentSaveFile.gameData.player_data = playerData;

        WriteDataToDisk();
    }

    string editorSaveFilePath = "Assets/ScriptableObjects/SaveFiles/";
    public void SaveGameEditor()
    {
#if UNITY_EDITOR
        SaveFileSO saveFileSO = ScriptableObject.CreateInstance<SaveFileSO>();

        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:SaveFileSO", null);
        string saveFileName = "SaveFile_" + guids.Length;
        string path = testSavePath + "/" + saveFileName;

        PlayerData playerData = GetPlayerData();
        SavablesToCache();

        saveFileSO.fileName = saveFileName;
        saveFileSO.path = path;
        saveFileSO.creationTime = Directory.GetCreationTime(path).ToShortDateString();
        saveFileSO.gameData = testSaveFile.gameData;

        string fileEditorPath = editorSaveFilePath + saveFileName + ".asset";

        UnityEditor.AssetDatabase.CreateAsset(saveFileSO, fileEditorPath);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();

        
#endif
    }


    private void SavablesToCache()
    {
        if (sceneDataCache.data_entries == null)
        {
            sceneDataCache.data_entries = new Dictionary<string, string>();
        }

        if (enemyDataCache.data_entries == null)
        {
            enemyDataCache.data_entries = new Dictionary<string, string>();
        }

        foreach (KeyValuePair<string, ISaveable> saveablePair in m_RegisteredSaveables)
        {
            if (saveablePair.Value is Entity)
            {
                enemyDataCache.data_entries[saveablePair.Key.ToString()] = saveablePair.Value.SaveData();
            }
            else
            {
                sceneDataCache.data_entries[saveablePair.Key.ToString()] = saveablePair.Value.SaveData();
            }
        }
    }

    public GameData LoadDataFromFile(string path)
    {
        GameData gameData = new GameData();

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            gameData = JsonConvert.DeserializeObject<GameData>(json); 
            return gameData;
        }
        else
        {
            return gameData;
        }
    }

    public void LoadSavedGame(SaveFileSO saveFile)
    {
        saveFiles.Clear();

        Debug.Log("Loading data from: " + saveFile.fileName);
        currentSaveFile = saveFile;

        SetPlayerData(saveFile);
        DataFileToCache();

        loadSaveFileEvent.Raise();
        StartCoroutine(IncrementTimePlayed());
    }

    public void LoadSavedGameFromSO(SaveFileSO saveFile)
    {
        saveFiles.Clear();

        Debug.Log("Loading data from SO: " + saveFile.fileName);
       
        string path = testSavePath + "/" + saveFile.fileName + ".json";
        saveFile.path = path;
        if (!File.Exists(path))
        {
            StreamWriter sw = File.CreateText(path);
            sw.Close();
        }

        currentSaveFile = saveFile;
        currentSaveFile.gameData = saveFile.gameData;
        sceneDataCache = currentSaveFile.gameData.scene_data;
        enemyDataCache = currentSaveFile.gameData.enemy_data;
        SetPlayerData(saveFile);

        if (sceneDataCache.data_entries == null)
        {
            sceneDataCache.data_entries = new Dictionary<string, string>();
        }
        if (enemyDataCache.data_entries == null)
        {
            enemyDataCache.data_entries = new Dictionary<string, string>();
        }

        if (m_RegisteredSaveables == null)
        {
            m_RegisteredSaveables = new Dictionary<string, ISaveable>();
        }

        loadSaveFileEvent.Raise();
        StartCoroutine(IncrementTimePlayed());
    }

    public void DeleteSaveFile(SaveSlot saveSlot, SaveFileSO saveFile)
    {
        string filePath = saveFile.path;

        if (!File.Exists(filePath))
        {
            Debug.Log(filePath + " does not exist exists.");
        }
        else
        {
            File.Delete(saveFile.path);
            Debug.Log(filePath + " deleted.");

        }

        saveSlot.saveFile = null;
        saveSlot.enabled = false;
        saveSlot.enabled = true;
    }

    public void NewGameData(int saveSlotID)
    {
        Debug.Log("Creating new save at slot with ID:" + saveSlotID);
        GameData gameData = DefaultGameData();

        string fileName = "GameSave_" + saveSlotID + ".json";
        string newGameSavePath = savePath + "/" + fileName;

        SaveFileSO saveFileSO = ScriptableObject.CreateInstance<SaveFileSO>();
        saveFileSO.fileName = fileName;
        saveFileSO.path = newGameSavePath;
        saveFileSO.slotIndex = saveSlotID;
        saveFileSO.creationTime = Directory.GetCreationTime(savePath).ToShortDateString();
        saveFileSO.gameData = gameData;

        currentSaveFile = saveFileSO;
        sceneDataCache = currentSaveFile.gameData.scene_data;
        enemyDataCache = currentSaveFile.gameData.enemy_data;

        WriteDataToDisk();

        //Setting up the GameManager Values
        SetPlayerData(saveFileSO);

        StartCoroutine(IncrementTimePlayed());
        newGameEvent.Raise();
    }

    private GameData DefaultGameData()
    {
        PlayerData playerData = DefaultPlayerData();

        SceneData sceneData = new SceneData
        {
            data_entries = new Dictionary<string, string>(),
        };

        EnemyData enemyCache = new EnemyData
        {
            data_entries = new Dictionary<string, string>(),
        };

        ItemData itemData = new ItemData
        {
            data_entries = new Dictionary<string, string>(),
        };

        GameData gameData = new GameData
        {
            player_data = playerData,
            scene_data = sceneData,
            enemy_data = enemyCache,
            item_data = itemData
        };

        return gameData;
    }

    private PlayerData GetPlayerData()
    {
        PlayerData playerData = currentSaveFile.gameData.player_data;

        playerData.maxHealth = playerDataSO.playerMaxHealth.Value;
        playerData.health = playerDataSO.playerHealth.Value;
        playerData.currency = playerDataSO.playerCurrency.Value;

        playerData.healingAmountPerPod = playerDataSO.playerHealingAmountPerPod.Value;
        playerData.healingPodAmount = playerDataSO.playerHealingPodAmount.Value;

        playerData.healthShardAmount = playerDataSO.playerHealthShardAmount.Value;
        playerData.healingShardAmount = playerDataSO.playerHealingFlaskShards.Value;

        playerData.healingPods = new int[playerData.healingPodAmount];
        for (int i = 0; i < playerData.healingPodAmount; i++)
        {
            playerData.healingPods[i] = playerDataSO.playerHealingPodFillAmounts[i];
        }

        playerData.playerPosition = new float[3];
        playerData.playerPosition[0] = playerDataSO.playerPosition.X;
        playerData.playerPosition[1] = playerDataSO.playerPosition.Y;
        playerData.playerPosition[2] = 0;

        playerData.lastCheckpointPos = new float[2];
        playerData.lastSavepointPos[0] = playerDataSO.lastSavepointPos.X;
        playerData.lastSavepointPos[1] = playerDataSO.lastSavepointPos.Y;

        playerData.lastCheckpointPos = new float[2];
        playerData.lastCheckpointPos[0] = playerDataSO.lastCheckpointPos.X;
        playerData.lastCheckpointPos[1] = playerDataSO.lastCheckpointPos.Y;

        playerData.lastCheckpointLevelIndex = playerDataSO.lastCheckpointLevelIndex.Value;
        playerData.lastSavepointLevelIndex = playerDataSO.lastSavepointLevelIndex.Value;

        playerData.lastCheckpointLevelPath = playerDataSO.lastCheckpointLevelPath;
        playerData.lastSavepointLevelPath = playerDataSO.lastSavepointLevelPath;

        playerData.currentScene = playerDataSO.currentSceneBuildIndex.Value;

        playerData.hasBoomerangAbility = playerDataSO.hasBoomerangAbility;
        playerData.hasDoubleJumpAbility = playerDataSO.hasDoubleJumpAbility;
        playerData.hasDashAbility = playerDataSO.hasDashAbility;
        playerData.hasTeleportAbility = playerDataSO.hasTeleportAbility;
        playerData.hasSprintAbility = playerDataSO.hasSprintAbility;
        playerData.hasWallJumpAbility = playerDataSO.hasWallJumpAbility;
        playerData.hasAirDashAbility = playerDataSO.hasAirDashAbility;

        return playerData;
    }


    private void SetPlayerData(SaveFileSO saveFile)
    {
        PlayerData data = saveFile.gameData.player_data;

        playerDataSO.playerHealth.Value = data.health;
        playerDataSO.playerMaxHealth.Value = data.maxHealth;
        playerDataSO.playerCurrency.Value = data.currency;

        playerDataSO.playerHealingPodAmount.Value = data.healingPodAmount;
        playerDataSO.playerHealingAmountPerPod.Value = data.healingAmountPerPod;

        playerDataSO.playerHealingFlaskShards.Value = data.healingShardAmount;
        playerDataSO.playerHealthShardAmount.Value = data.healthShardAmount;

        playerDataSO.playerHealingPodFillAmounts.Clear();
        for (int i = 0; i < data.healingPodAmount; i++)
        {
            playerDataSO.playerHealingPodFillAmounts.Add(data.healingPods[i]);
        }

        Vector3 playerPosition;
        playerPosition.x = data.playerPosition[0];
        playerPosition.y = data.playerPosition[1];
        playerPosition.z = data.playerPosition[2];

        playerDataSO.playerPosition.X = playerPosition.x;
        playerDataSO.playerPosition.Y = playerPosition.y;

        Vector2 checkpointPosition;
        checkpointPosition.x = data.lastCheckpointPos[0];
        checkpointPosition.y = data.lastCheckpointPos[1];
        playerDataSO.lastCheckpointPos.X = checkpointPosition.x;
        playerDataSO.lastCheckpointPos.Y = checkpointPosition.y;

        playerDataSO.lastCheckpointLevelIndex.Value = data.lastCheckpointLevelIndex;
        playerDataSO.lastCheckpointLevelPath = data.lastCheckpointLevelPath;

        Vector2 savePointPosition;
        savePointPosition.x = data.lastSavepointPos[0];
        savePointPosition.y = data.lastSavepointPos[1];
        playerDataSO.lastSavepointPos.X = savePointPosition.x;
        playerDataSO.lastSavepointPos.Y = savePointPosition.y;

        playerDataSO.lastSavepointLevelIndex.Value = data.lastSavepointLevelIndex;
        playerDataSO.lastSavepointLevelPath = data.lastSavepointLevelPath;

        playerDataSO.currentSceneBuildIndex.Value = data.currentScene;

        playerDataSO.hasBoomerangAbility = data.hasBoomerangAbility;
        playerDataSO.hasDoubleJumpAbility = data.hasDoubleJumpAbility;
        playerDataSO.hasDashAbility = data.hasDashAbility;
        playerDataSO.hasTeleportAbility = data.hasTeleportAbility;
        playerDataSO.hasSprintAbility = data.hasSprintAbility;
        playerDataSO.hasWallJumpAbility = data.hasWallJumpAbility;
        playerDataSO.hasAirDashAbility = data.hasAirDashAbility;
    }

    private PlayerData DefaultPlayerData()
    {
        PlayerData playerData = new PlayerData();
        Vector3 initialPosition = initialPlayerData.initialPlayerPosition;

        playerData.health = initialPlayerData.health;
        playerData.maxHealth = initialPlayerData.maxHealth;
        playerData.currency = initialPlayerData.initialCurrency;

        playerData.healingShardAmount = initialPlayerData.healingShardAmount;
        playerData.healthShardAmount = initialPlayerData.healthShardAmount;

        playerData.healingAmountPerPod = initialPlayerData.healingAmountPerPod;
        playerData.healingPodAmount = initialPlayerData.healinPodAmount;
        playerData.healingPods = new int[initialPlayerData.healinPodAmount];

        for (int i = 0; i < playerData.healingPodAmount; i++)
        {
            playerData.healingPods[i] = initialPlayerData.healingPods[i];
        }

        playerData.playerPosition = new float[3];
        playerData.playerPosition[0] = initialPosition.x;
        playerData.playerPosition[1] = initialPosition.y;
        playerData.playerPosition[2] = initialPosition.z;

        playerData.lastCheckpointPos = new float[2];
        playerData.lastSavepointPos[0] = initialPosition.x;
        playerData.lastSavepointPos[1] = initialPosition.y;

        playerData.lastCheckpointPos = new float[2];
        playerData.lastCheckpointPos[0] = initialPosition.x;
        playerData.lastCheckpointPos[1] = initialPosition.y;

        playerData.lastCheckpointLevelIndex = initialPlayerData.initialLevelIndex;
        playerData.lastSavepointLevelIndex = initialPlayerData.initialLevelIndex;

        playerData.lastCheckpointLevelPath = initialPlayerData.initialLevelPath;
        playerData.lastSavepointLevelPath = initialPlayerData.initialLevelPath;

        playerData.currentScene = initialPlayerData.initialLevelIndex;

        playerData.hasBoomerangAbility = false;
        playerData.hasDoubleJumpAbility = false;
        playerData.hasDashAbility = true;
        playerData.hasTeleportAbility = false;
        playerData.hasSprintAbility = false;
        playerData.hasWallJumpAbility = false;
        playerData.hasAirDashAbility = false;

        return playerData;
    }

    private IEnumerator IncrementTimePlayed()
    {
        WaitForSeconds incrementSecond = new WaitForSeconds(1);

        while (true)
        {
            yield return incrementSecond;

            if (currentSaveFile.gameData != null)
            {
                currentSaveFile.gameData.timePlayed = currentSaveFile.gameData.timePlayed.Add(TimeSpan.FromSeconds(1));
            }
        }
    }
}

[System.Serializable]
public class GameData
{
    [Serializable]
    public struct MetaData
    {
        public int gameVersion;
        public string creationDate;
        public string timePlayed;
    }

    [NonSerialized] public TimeSpan timePlayed;
    [NonSerialized] public int gameVersion;
    [NonSerialized] public DateTime creationDate;

    public MetaData metaData;
    public PlayerData player_data;
    public SceneData scene_data;
    public EnemyData enemy_data;
    public ItemData item_data;

    public void OnWrite()
    {
        if (creationDate == default(DateTime))
        {
            creationDate = DateTime.Now;
        }

        metaData.creationDate = creationDate.ToString();
        metaData.gameVersion = gameVersion;
        metaData.timePlayed = timePlayed.ToString();
    }

    public void OnLoad()
    {
        gameVersion = metaData.gameVersion;

        DateTime.TryParse(metaData.creationDate, out creationDate);
        TimeSpan.TryParse(metaData.timePlayed, out timePlayed);
    }
}

[System.Serializable]
public class SceneData
{
    public Dictionary<string, string> data_entries;
}

[System.Serializable]
public class EnemyData
{
    public Dictionary<string, string> data_entries;
}

[System.Serializable]
public class ItemData
{
    public Dictionary<string, string> data_entries;
}

[System.Serializable]
public class DataEntry
{
    public string key;
    public string data;
}

