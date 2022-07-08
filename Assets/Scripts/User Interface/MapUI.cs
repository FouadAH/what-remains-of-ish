using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapUI : MonoBehaviour
{
    public GameObject mapUI;
    public MapSystem mapSystem;

    PlayerInputMaster inputActions;

    private void Start()
    {
        inputActions = FindObjectOfType<Player_Input>().inputActions;
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
