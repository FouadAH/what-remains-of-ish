using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameMenu : MonoBehaviour
{
    public GameObject mapUI;

    PlayerInputMaster inputActions;

    private void Start()
    {
        inputActions = new PlayerInputMaster();
        inputActions.Player.Enable();
        inputActions.UI.Enable();

        inputActions.UI.Map.started += Map_started;
        inputActions.UI.Map.canceled += Map_canceled;
    }

    private void Map_canceled(InputAction.CallbackContext obj)
    {
        CloseMap();
    }

    private void Map_started(InputAction.CallbackContext obj)
    {
        OpenMap();
    }


    void OpenMap()
    {
        mapUI.SetActive(true);
    }

    void CloseMap()
    {
        mapUI.SetActive(false);
    }

    private void OnDestroy()
    {
        inputActions.UI.Map.started -= Map_started;
        inputActions.UI.Map.canceled -= Map_canceled;
    }

}
