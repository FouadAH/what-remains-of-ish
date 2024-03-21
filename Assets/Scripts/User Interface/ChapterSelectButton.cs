using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterSelectButton : MonoBehaviour
{
    public SaveFileSO saveFile;

    public void OnClickChapter()
    {
        GetComponent<Button>().onClick.AddListener(() => SaveManager.instance.LoadSavedGameFromSO(saveFile));
    }

    public void LoadSaveFile()
    {
        SaveManager.instance.LoadSavedGameFromSO(saveFile);
    }
}
