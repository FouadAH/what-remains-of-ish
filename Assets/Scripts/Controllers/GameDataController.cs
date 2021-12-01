using System.IO;
using UnityEngine;

public class GameDataController
{
    public static PlayerData saveData;
    
    [ContextMenu("Save Data")]
    public static void SaveGame()
    {
        saveData = new PlayerData(GameManager.instance);
        var data = JsonUtility.ToJson(saveData);
        string path = Application.persistentDataPath + "/SaveFiles/player.json";
        StreamWriter sw = File.CreateText(path);
        sw.Close();
        File.WriteAllText(path, data);
        Debug.Log("Saved Game Data to: " + path);
    }

    [ContextMenu("Load Data")]
    public static PlayerData LoadData()
    {
        string path = Application.persistentDataPath + "/SaveFiles/player.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return saveData = JsonUtility.FromJson<PlayerData>(json);
        }
        else
        {
            Debug.LogError("No save file found at " + path);
            return null;
        }
    }

    public static PlayerData LoadData(string path)
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return saveData = JsonUtility.FromJson<PlayerData>(json);
        }
        else
        {
            Debug.LogError("No save file found at " + path);
            return null;
        }
    }

}
