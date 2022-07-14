using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SaveSlotController : MonoBehaviour
{
    public List<SaveSlot> saveSlots;
    List<SaveFileSO> saveFiles;
    public Canvas deletePrompt;

    SaveSlot currentSaveSlot;
    SaveFileSO currentSaveFile;

    private void Start()
    {
        SaveManager.instance.InitSaveManager();

        saveFiles = SaveManager.instance.saveFiles;

        //Assigning save files to the save slots
        for (int i = 0; i < saveFiles.Count; i++)
        {
            saveSlots[saveFiles[i].slotIndex].saveFile = saveFiles[i];
            Debug.Log("Save file assigned to Slot with ID: " + saveFiles[i].slotIndex);
        }

        //Enabling the save slot displays
        foreach (SaveSlot saveSlot in saveSlots)
        {
            saveSlot.enabled = true;
            saveSlot.gameObject.SetActive(true);

            if (saveSlot.isPreSaved)
            {
                saveSlot.GetComponent<Button>().onClick.AddListener(
                    () => SaveManager.instance.LoadSavedGameFromSO(saveSlot.saveFile));

                saveSlot.GetComponent<Image>().color = Color.red;
                saveSlot.areaName.text = saveSlot.saveFile.fileName;
                saveSlot.deleteButton.SetActive(false);
            }
            else if (saveSlot.saveFile == null)
            {
                saveSlot.GetComponent<Button>().onClick.AddListener(
                    () => SaveManager.instance.NewGameData(saveSlot.transform.GetSiblingIndex()));
                saveSlot.areaName.text = "New Game";
            }
            else
            {
                saveSlot.GetComponent<Button>().onClick.AddListener(
                    () => SaveManager.instance.LoadSavedGame(saveSlot.saveFile));

                saveSlot.deleteButton.GetComponent<Button>().onClick.AddListener(
                    () => DeletePrompt(saveSlot, saveSlot.saveFile));
            }
        }
    }

    public void DeletePrompt(SaveSlot saveSlot, SaveFileSO saveFile)
    {
        EventSystem.current.SetSelectedGameObject(deletePrompt.GetComponentInChildren<Button>().gameObject);

        currentSaveSlot = saveSlot;
        currentSaveFile = saveFile;
        deletePrompt.enabled = true;
    }

    public void OnClickYes()
    {
        currentSaveSlot.GetComponent<Button>().onClick.RemoveAllListeners();
        currentSaveSlot.GetComponent<Button>().onClick.AddListener(() => SaveManager.instance.NewGameData(currentSaveSlot.transform.GetSiblingIndex()));

        SaveManager.instance.DeleteSaveFile(currentSaveSlot, currentSaveFile);
        deletePrompt.enabled = false;

        EventSystem.current.SetSelectedGameObject(GetComponentInChildren<Button>().gameObject);

    }

    public void OnClickNo()
    {
        deletePrompt.enabled = false;

        EventSystem.current.SetSelectedGameObject(GetComponentInChildren<Button>().gameObject);
    }

}