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
    public GameEvent saveDataEvent;
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

#if UNITY_EDITOR
        if (!Directory.Exists(testSavePath))
        {
            Directory.CreateDirectory(testSavePath);
        }
#endif

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
#if UNITY_EDITOR
        SetupTestingSaveFile();
#endif

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
    }


    public void WriteDataToDisk()
    {
        currentSaveFile.gameData.scene_data = sceneDataCache;
        currentSaveFile.gameData.enemy_data = enemyDataCache;

        currentSaveFile.gameData.OnWrite();

        string data = JsonConvert.SerializeObject(currentSaveFile.gameData); 
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
        saveFiles = null;

        Debug.Log("Loading data from: " + saveFile.fileName);
        currentSaveFile = saveFile;

        SetPlayerData(saveFile);
        DataFileToCache();

        loadSaveFileEvent.Raise();
        StartCoroutine(IncrementTimePlayed());
    }

    public void LoadSavedGameFromSO(SaveFileSO saveFile)
    {
        saveFiles = null;

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
            sceneDataCache.data_entries = new Dictionary<string, string>();
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

        GameData gameData = new GameData
        {
            player_data = playerData,
            scene_data = sceneData,
            enemy_data = enemyCache
        };
        return gameData;
    }

    private PlayerData GetPlayerData()
    {
        PlayerData playerData = currentSaveFile.gameData.player_data;
        GameManager gm = GameManager.instance;

        playerData.maxHealth = gm.maxHealth;
        playerData.health = gm.health;
        playerData.currency = gm.currency;

        playerData.healingPodAmount = gm.healingPodAmount;
        playerData.healingPods = new int[playerData.healingPodAmount];
        for (int i = 0; i < playerData.healingPodAmount; i++)
        {
            playerData.healingPods[i] = gm.healingPodFillAmounts[i];
        }

        playerData.playerPosition = new float[3];
        playerData.playerPosition[0] = gm.playerCurrentPosition.position.x;
        playerData.playerPosition[1] = gm.playerCurrentPosition.position.y;
        playerData.playerPosition[2] = gm.playerCurrentPosition.position.z;

        playerData.lastCheckpointPos = new float[2];
        playerData.lastSavepointPos[0] = gm.lastSavepointPos.x;
        playerData.lastSavepointPos[1] = gm.lastSavepointPos.y;

        playerData.lastCheckpointPos = new float[2];
        playerData.lastCheckpointPos[0] = gm.lastCheckpointPos.x;
        playerData.lastCheckpointPos[1] = gm.lastCheckpointPos.y;

        playerData.lastCheckpointLevelIndex = gm.lastCheckpointLevelIndex;
        playerData.lastSavepointLevelIndex = gm.lastSavepointLevelIndex;

        playerData.lastCheckpointLevelPath = gm.lastCheckpointLevelPath;
        playerData.lastSavepointLevelPath = gm.lastSavepointLevelPath;

        playerData.currentScene = gm.currentSceneBuildIndex;
        return playerData;
    }


    private static void SetPlayerData(SaveFileSO saveFile)
    {
        PlayerData data = saveFile.gameData.player_data;

        GameManager.instance.health = data.health;
        GameManager.instance.maxHealth = data.maxHealth;
        GameManager.instance.currency = data.currency;

        GameManager.instance.healingPodAmount = data.healingPodAmount;
        for (int i = 0; i < data.healingPodAmount; i++)
        {
            GameManager.instance.healingPodFillAmounts.Add(data.healingPods[i]);
        }

        Vector3 playerPosition;
        playerPosition.x = data.playerPosition[0];
        playerPosition.y = data.playerPosition[1];
        playerPosition.z = data.playerPosition[2];
        GameManager.instance.playerStartPosition = playerPosition;

        Vector2 checkpointPosition;
        checkpointPosition.x = data.lastCheckpointPos[0];
        checkpointPosition.y = data.lastCheckpointPos[1];
        GameManager.instance.lastCheckpointPos = checkpointPosition;

        GameManager.instance.lastCheckpointLevelIndex = data.lastCheckpointLevelIndex;
        GameManager.instance.lastCheckpointLevelPath = data.lastCheckpointLevelPath;

        Vector2 savePointPosition;
        savePointPosition.x = data.lastSavepointPos[0];
        savePointPosition.y = data.lastSavepointPos[1];
        GameManager.instance.lastSavepointPos = savePointPosition;

        GameManager.instance.lastSavepointLevelIndex = data.lastSavepointLevelIndex;
        GameManager.instance.lastSavepointLevelPath = data.lastSavepointLevelPath;

        GameManager.instance.currentSceneBuildIndex = data.currentScene;
    }

    private PlayerData DefaultPlayerData()
    {
        PlayerData playerData = new PlayerData();
        Vector3 initialPosition = initialPlayerData.initialPlayerPosition;

        playerData.health = initialPlayerData.health;
        playerData.maxHealth = initialPlayerData.maxHealth;
        playerData.currency = initialPlayerData.initialCurrency;

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
public class DataEntry
{
    public string key;
    public string data;
}

