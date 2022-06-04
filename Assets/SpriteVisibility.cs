using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteVisibility : MonoBehaviour
{
    private void OnBecameInvisible()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    private void OnBecameVisible()
    {
        GetComponent<SpriteRenderer>().enabled = true;
    }
}
