using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintTrigger : MonoBehaviour
{
    [TextArea(3, 10)]
    public string hintText;
    bool hasBeenActivated = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasBeenActivated)
            return;

        if (collision.GetComponent<Player>())
        {
            hasBeenActivated = true;
            UI_HUD.instance.SetTipsText(hintText);
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Interactive Objects/Hints", GetComponent<Transform>().position);
        }
    }

}
