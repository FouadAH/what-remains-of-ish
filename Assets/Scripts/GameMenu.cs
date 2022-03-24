using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameMenu : MonoBehaviour
{
    PlayerInputMaster inputActions;

    public Canvas gameMenu;
    public GameObject mapScreen;
    public GameObject broochesScreen;
    public GameObject inventoryScreen;
    public GameObject journalScreen;

    public GameObject mapButton;
    public GameObject broochesButton;
    public GameObject inventoryButton;
    public GameObject journalButton;

    EventSystem eventSystem;

    private void Start()
    {
        eventSystem = EventSystem.current;
        inputActions = FindObjectOfType<Player_Input>().inputActions;
        inputActions.UI.GameMenu.started += GameMenu_started;
    }

    private void GameMenu_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (gameMenu.enabled)
        {
            gameMenu.enabled = false;
            DialogManager.instance.dialogueIsActive = false;
            DialogManager.instance.OnInteractEnd();
            OnClickInventory();
        }
        else
        {
            eventSystem.firstSelectedGameObject = inventoryButton;
            eventSystem.SetSelectedGameObject(inventoryButton);
            gameMenu.enabled = true;
            DialogManager.instance.dialogueIsActive = true;
            DialogManager.instance.OnInteractStart();
        }
    }

    public void OnClickMap()
    {
        mapScreen.SetActive(true);
        broochesScreen.SetActive(false);
        inventoryScreen.SetActive(false);
        journalScreen.SetActive(false);
    }

    public void OnClickInventory()
    {
        mapScreen.SetActive(false);
        broochesScreen.SetActive(false);
        inventoryScreen.SetActive(true);
        journalScreen.SetActive(false);
    }

    public void OnClickBrooches()
    {
        mapScreen.SetActive(false);
        broochesScreen.SetActive(true);
        inventoryScreen.SetActive(false);
        journalScreen.SetActive(false);
    }

    public void OnClickJournal()
    {
        mapScreen.SetActive(false);
        broochesScreen.SetActive(false);
        inventoryScreen.SetActive(false);
        journalScreen.SetActive(true);
    }
}
