using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretShadowController : MonoBehaviour
{

    Animator anim;
    bool isRevealed = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(isRevealed)
        {
            return;
        }

        if(collision.GetComponent<Player>() != null)
        {
            anim.Play("SecretRoomReveal");
            isRevealed = true;
        }
    }
}
