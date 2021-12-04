using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class SaveSlotController : MonoBehaviour
{
    public List<SaveSlot> saveSlots;
    List<SaveFileSO> saveFiles;

    private void Start()
    {
        saveFiles = SaveManager.instance.saveFiles;

        //Assigning save files to the save slots
        for (int i = 0; i < saveFiles.Count; i++)
        {
            saveSlots[i].saveFile = saveFiles[i];
        }

        //Enabaling the save slot displays
        foreach (SaveSlot saveSlot in saveSlots)
        {
            saveSlot.enabled = true;
            saveSlot.gameObject.SetActive(true);

            if (saveSlot.saveFile == null)
            {
                saveSlot.GetComponent<Button>().onClick.AddListener(() => SaveManager.instance.NewGameData(saveSlot.transform.GetSiblingIndex()));
            }
            else
            {
                saveSlot.GetComponent<Button>().onClick.AddListener(() => SaveManager.instance.LoadSavedGame(saveSlot.saveFile));
            }
        }
    }
}
