using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newSaveFile", menuName = "New Save File", order = 1)]
public class SaveFileSO : ScriptableObject
{
    public string fileName;
    public string path;
    public PlayerData playerData;

    public SaveFileSO(string fileName, string filePath, PlayerData playerData)
    {
        this.fileName = fileName;
        this.path = filePath;
        this.playerData = playerData;
    }
}
