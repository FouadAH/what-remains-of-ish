using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupTrigger : MonoBehaviour
{
    [TextArea(3, 10)]
    public string popUpText;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            UI_HUD.instance.SetDebugText(popUpText);
        }
    }
}
