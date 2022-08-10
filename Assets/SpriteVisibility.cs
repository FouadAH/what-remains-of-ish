using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteVisibility : MonoBehaviour
{
    private void OnBecameInvisible()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    private void OnBecameVisible()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
