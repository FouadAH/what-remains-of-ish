using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintTrigger : MonoBehaviour
{
    [TextArea(3, 10)]
    public string hintText;
    [FMODUnity.EventRef] 
    public string hintSFX;
    public StringEvent hintTextEvent;

    bool hasBeenActivated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasBeenActivated)
            return;

        if (collision.GetComponent<Player>())
        {
            hasBeenActivated = true;
            hintTextEvent.Raise(hintText);
            FMODUnity.RuntimeManager.PlayOneShot(hintSFX, transform.position);
        }
    }

}
