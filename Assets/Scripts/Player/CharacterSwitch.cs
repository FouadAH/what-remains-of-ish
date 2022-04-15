using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSwitch : MonoBehaviour
{
    //public GameObject playerGirl;
    //public GameObject playerBoy;

    //Player_Input mainPlayerInput;

    //public GameObject[] players;
    //public GameObject currentPlayer;
    //int currentPlayerIndex;

    //public CameraController cameraController;
    //public Camera mainCamera;

    //bool isDetached = false;

    //private void Start()
    //{
    //    mainPlayerInput = playerGirl.GetComponent<Player_Input>();
    //    mainPlayerInput.OnDetach += DetachBoy;
    //    mainPlayerInput.OnAttach += ReattachBoy;

    //    mainCamera = GameManager.instance.playerCamera;
    //    cameraController = GameManager.instance.cameraController;

    //    GameManager.instance.player = players[0];
    //    players[0].GetComponent<Player_Input>().enabled = true;
    //}

    //private void Update()
    //{
    //    if (!isDetached)
    //        return;

    //    if (Input.GetKeyDown(KeyCode.P))
    //    {
    //        if(currentPlayerIndex == 0)
    //        {
    //            EnableBoy();
    //        }
    //        else
    //        {
    //            EnableGirl();
    //        }
    //    }
    //}

    //public void DetachBoy()
    //{
    //    if (isDetached)
    //        return;

    //    isDetached = true;
    //    playerBoy.transform.position = playerGirl.transform.position;
    //    playerBoy.transform.rotation = playerGirl.transform.rotation;
    //    players[1] = playerBoy;

    //    playerBoy.SetActive(true);

    //    EnableBoy();
    //    DisableGirl();
    //}

    //public void ReattachBoy()
    //{
    //    if (!isDetached)
    //        return;

    //    isDetached = false;
    //    EnableGirl();
    //    EnableBoyGirl();
    //    playerBoy.SetActive(false);
    //}

    //void EnableBoy()
    //{
    //    players[0].GetComponent<Player_Input>().enabled = false;
    //    SwitchCharacter(1);
    //}

    //void EnableGirl()
    //{
    //    players[1].GetComponent<Player_Input>().enabled = false;
    //    SwitchCharacter(0);
    //}

    //void DisableGirl()
    //{
    //    players[0].GetComponent<PlayerTeleport>().enabled = false;
    //    players[0].GetComponentInChildren<BoomerangLauncher>().enabled = false;
    //}

    //void EnableBoyGirl()
    //{
    //    players[0].GetComponent<PlayerTeleport>().enabled = true;
    //    players[0].GetComponentInChildren<BoomerangLauncher>().enabled = true;
    //}

    //public void SwitchCharacter(int playerIndex)
    //{ 
    //    players[playerIndex].GetComponent<Player_Input>().enabled = true;

    //    currentPlayer = players[playerIndex];
    //    currentPlayerIndex = playerIndex;

    //    mainPlayerInput = players[playerIndex].GetComponent<Player_Input>();
    //    mainPlayerInput.OnDetach += DetachBoy;
    //    mainPlayerInput.OnAttach += ReattachBoy;

    //    GameManager.instance.player = players[playerIndex];
    //    GameManager.instance.cameraController.virtualCamera.Follow = currentPlayer.transform;
    //}
}
