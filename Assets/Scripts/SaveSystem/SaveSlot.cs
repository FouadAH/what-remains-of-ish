using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SaveSlot : MonoBehaviour
{
    public SaveFileSO saveFile;
    public TMP_Text areaName;
    public TMP_Text timeElapsed;
    public TMP_Text progressPercentage;

    private void OnEnable()
    {
        if(saveFile != null) 
        {
            areaName.text = "Save Slot " + (transform.GetSiblingIndex() + 1);
            timeElapsed.text = saveFile.gameData.metaData.timePlayed;
            progressPercentage.text = saveFile.gameData.metaData.creationDate;
        }
        else
        {
            areaName.text = "Empty Slot";
            timeElapsed.text = "00:00";
            progressPercentage.text = "";
        }
    }
}