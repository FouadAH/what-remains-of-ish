using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapUI : MonoBehaviour
{
    public GameObject mapUI;
    public MapSystem mapSystem;

    PlayerInputMaster inputActions;
    Player_Input playerInput;

    private void Start()
    {
        playerInput = FindObjectOfType<Player_Input>();
        inputActions = playerInput.inputActions;
        inputActions.Player.Enable();
        inputActions.UI.Enable();

        inputActions.UI.Map.started += Map_started;
        inputActions.UI.Map.canceled += Map_canceled;
    }

    private void Map_canceled(InputAction.CallbackContext obj)
    {
        CloseMap();

        if (!GameManager.instance.isPaused)
        {
            playerInput.EnablePlayerInput();
        }
    }

    private void Map_started(InputAction.CallbackContext obj)
    {
        if (!GameManager.instance.isPaused)
        {
            OpenMap();
            playerInput.DisablePlayerInput();
        }
    }


    public void OpenMap()
    {
        mapUI.SetActive(true);
        mapSystem.OpenMap();
    }

    public void CloseMap()
    {
        mapUI.SetActive(false);
    }

    private void OnDestroy()
    {
        inputActions.UI.Map.started -= Map_started;
        inputActions.UI.Map.canceled -= Map_canceled;
    }

}
