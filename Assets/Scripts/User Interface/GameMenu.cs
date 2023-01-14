using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    PlayerInputMaster inputActions;

    public Canvas gameMenu;
    public GameObject mapScreen;
    public GameObject broochesScreen;
    public GameObject inventoryScreen;
    public GameObject journalScreen;
    public GameObject settingsScreen;

    public Color activeColor = Color.white;
    public Color inactiveColor = Color.black;

    public Transform navButtonsParent;
    public Image mapButton;
    public Image broochesButton;
    public Image inventoryButton;
    public Image journalButton;
    public Image settingsButton;

    public GameEvent pauseClicked;

    EventSystem eventSystem;

    int menuIndex = -1;
    int totalMenuNumber;

    int menuNavigationDirection;

    private void Start()
    {
        eventSystem = EventSystem.current;
        inputActions = FindObjectOfType<Player_Input>().inputActions;
        inputActions.UI.GameMenu_Navigate.started += GameMenu_Navigate_started;
        inputActions.UI.Pause.started += Pause_started;

        for (int i = 0; i < navButtonsParent.childCount; i++)
        {
            if (navButtonsParent.GetChild(i).gameObject.activeSelf)
            {
                totalMenuNumber++;
            }
        }
    }

    private void Pause_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        TogglePause();
        pauseClicked.Raise();
    }

    private void GameMenu_Navigate_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!gameMenu.enabled)
        {
            return;
        }

        menuNavigationDirection = (int)inputActions.UI.GameMenu_Navigate.ReadValue<float>();

        if (menuNavigationDirection > 0)
        {
            RightArrow();
        }
        else if (menuNavigationDirection < 0)
        {
            LeftArrow();
        }
    }

    public void TogglePause()
    {
        if (ShopManager.instance.shopIsActive)
            return;

        if (GameManager.instance.isPaused)
            Resume();
        else
            Pause();

        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/UI/Pause Menu/Pause Button", GetComponent<Transform>().position);
    }

    GameObject currentMenu;
    void Pause()
    {
        if(currentMenu == null)
        {
            OnClickSettings();
            menuIndex = 3;
        }

        SwitchMenu();
        gameMenu.enabled = true;

        GameManager.instance.isPaused = true;
        Time.timeScale = 0;

        if (!DialogManager.instance.dialogueIsActive && !CutsceneManager.instance.isCutscenePlaying)
            inputActions.Player.Disable();
    }

    public void Resume()
    {
        gameMenu.enabled = false;
        DisableAllMenus();

        GameManager.instance.isPaused = false;
        Time.timeScale = 1;

        if (!DialogManager.instance.dialogueIsActive && !CutsceneManager.instance.isCutscenePlaying)
            inputActions.Player.Enable();
    }

    public void LeftArrow()
    {
        if (totalMenuNumber > 1)
        {
            menuIndex--;

            if (menuIndex < 0)
            {
                menuIndex = totalMenuNumber - 1;
            }

            SwitchMenu();
        }
    }

    public void RightArrow()
    {
        if (totalMenuNumber > 1)
        {
            menuIndex++;
            SwitchMenu();
        }
    }

    public void SwitchMenu()
    {
        if (totalMenuNumber > 1)
        {
            menuIndex %= totalMenuNumber;
        }

        switch (menuIndex)
        {
            case 0:
                OnClickMap();
                break;
            case 1:
                OnClickBrooches();
                break;
            case 2:
                OnClickJournal();
                break;
            case 3:
                OnClickSettings();
                break;
            default:
                break;
        }
    }

    void DeselectButtons()
    {
        settingsButton.color = inactiveColor;
        broochesButton.color = inactiveColor;
        journalButton.color = inactiveColor;
        inventoryButton.color = inactiveColor;
        mapButton.color = inactiveColor;
    }

    void DisableAllMenus()
    {
        settingsScreen.SetActive(false);
        broochesScreen.SetActive(false);
        inventoryScreen.SetActive(false);
        journalScreen.SetActive(false);
        mapScreen.SetActive(false);
    }

    public void OnClickSettings()
    {
        currentMenu = settingsScreen;

        DeselectButtons();

        settingsButton.color = activeColor;

        settingsScreen.SetActive(true);
        broochesScreen.SetActive(false);
        inventoryScreen.SetActive(false);
        journalScreen.SetActive(false);
        mapScreen.SetActive(false);

        eventSystem.SetSelectedGameObject(settingsScreen.GetComponentInChildren<Button>().gameObject);

    }

    public void OnClickMap()
    {
        currentMenu = mapScreen;

        DeselectButtons();
        mapButton.color = activeColor;

        mapScreen.SetActive(true);
        broochesScreen.SetActive(false);
        inventoryScreen.SetActive(false);
        journalScreen.SetActive(false);
        settingsScreen.SetActive(false);
    }

    public void OnClickInventory()
    {
        currentMenu = inventoryScreen;

        DeselectButtons();
        inventoryButton.color = activeColor;

        mapScreen.SetActive(false);
        broochesScreen.SetActive(false);
        inventoryScreen.SetActive(true);
        journalScreen.SetActive(false);
        settingsScreen.SetActive(false);
    }

    public void OnClickBrooches()
    {
        currentMenu = broochesScreen;

        DeselectButtons();
        broochesButton.color = activeColor;

        mapScreen.SetActive(false);
        broochesScreen.SetActive(true);
        inventoryScreen.SetActive(false);
        journalScreen.SetActive(false);
        settingsScreen.SetActive(false);

        //broochesScreen.GetComponent<InventoryDragDropSystem>().Setup();
        GameObject broocheGO = broochesScreen.GetComponent<InventoryDragDropSystem>().GetFirstItem();
        eventSystem.firstSelectedGameObject = broocheGO;
        eventSystem.SetSelectedGameObject(broocheGO);
    }

    public void OnClickJournal()
    {
        currentMenu = journalScreen;

        DeselectButtons();
        journalButton.color = activeColor;

        mapScreen.SetActive(false);
        broochesScreen.SetActive(false);
        inventoryScreen.SetActive(false);
        settingsScreen.SetActive(false);

        journalScreen.SetActive(true);
    }

    private void OnDestroy()
    {
        inputActions.UI.GameMenu_Navigate.started -= GameMenu_Navigate_started;
        inputActions.UI.Pause.started -= Pause_started;
    }
}
