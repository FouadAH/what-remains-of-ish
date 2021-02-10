using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Cinemachine.CinemachineBrain cinemachineBrain;
    public Cinemachine.CinemachineVirtualCamera virtualCamera;
    public Cinemachine.CinemachineConfiner confiner;

    //script on camera
    public void ResetPosition()
    {
        //have to disable cinemachine for some time otherwise it will not set the position and will smooth follow to the position
        cinemachineBrain.enabled = false;
        virtualCamera.enabled = false;
        //cinemachineBrain.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, cinemachineBrain.transform.position.z);
       // virtualCamera.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, virtualCamera.transform.position.z);

        StartCoroutine(DelayCinemachine(0.05f));
    }

    IEnumerator DelayCinemachine(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        cinemachineBrain.enabled = true;
        virtualCamera.enabled = true;
    }
}
