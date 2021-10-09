using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretRoomShadow : MonoBehaviour
{
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            animator.Play("SecretRoomReveal");
        }
    }
}
