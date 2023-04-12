using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DemoMainMenu : MonoBehaviour
{
    public GameObject titleScreen;
    public GameObject chapterSelect;

    public void ChapterSelectOpen()
    {
        titleScreen.SetActive(false);
        chapterSelect.SetActive(true);
        EventSystem.current.SetSelectedGameObject(chapterSelect.GetComponentInChildren<Button>().gameObject);
    }
    public void ChapterSelectClose()
    {
        titleScreen.SetActive(true);
        chapterSelect.SetActive(false);
        EventSystem.current.SetSelectedGameObject(titleScreen.GetComponentInChildren<Button>().gameObject);
    }
}
