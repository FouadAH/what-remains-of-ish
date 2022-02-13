using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenu : MonoBehaviour
{
    PlayerInputMaster inputActions;

    public Canvas gameMenu;
    public GameObject mapScreen;
    public GameObject broochesScreen;
    public GameObject inventoryScreen;

    private void Start()
    {
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
    }

    public void OnClickInventory()
    {
        mapScreen.SetActive(false);
        broochesScreen.SetActive(false);
        inventoryScreen.SetActive(true);
    }

    public void OnClickBrooches()
    {
        mapScreen.SetActive(false);
        broochesScreen.SetActive(true);
        inventoryScreen.SetActive(false);
    }
}
