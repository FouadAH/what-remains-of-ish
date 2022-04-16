using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newSaveFile", menuName = "New Save File", order = 1)]
public class SaveFileSO : ScriptableObject
{
    public string fileName;
    public string path;
    public string creationTime;
    public GameData gameData;
}
