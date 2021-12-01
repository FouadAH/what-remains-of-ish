using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class LoadView : MonoBehaviour
{
    public List<SaveFileSO> saveFiles;
    public GameObject saveFilesParent;
    public GameObject saveButton;
    public GameEvent loadSaveFileEvent;

    string savePath;  //TO:DO : get this from config file

    private void Awake()
    {
        savePath = Application.persistentDataPath + "/SaveFiles";
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
    }
    private void Start()
    {
        foreach(string filePath in Directory.GetFiles(savePath))
        {
            PlayerData playerData = GameDataController.LoadData(filePath);
            SaveFileSO saveFileSO = new SaveFileSO(Path.GetFileName(filePath), filePath, playerData);
            saveFiles.Add(saveFileSO);
            Directory.GetCreationTime(filePath).ToShortDateString();

            //Creating a button
            GameObject saveFileView = Instantiate(saveButton, saveFilesParent.transform);
            TMP_Text[] saveViewTexts = saveFileView.GetComponentsInChildren<TMP_Text>();
            saveViewTexts[0].text = saveFileSO.fileName;
            saveViewTexts[2].text = Directory.GetCreationTime(filePath).ToShortDateString();

            saveFileView.GetComponent<Button>().onClick.AddListener(() => RaiseLoadSaveFileEvent(filePath));
        }
    }

    void RaiseLoadSaveFileEvent(string filePath)
    {
        loadSaveFileEvent.Raise();
    }

}
