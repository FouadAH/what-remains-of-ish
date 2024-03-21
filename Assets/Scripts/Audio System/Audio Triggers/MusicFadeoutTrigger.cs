using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicFadeoutTrigger : MonoBehaviour
{
    BoxCollider2D boxCollider2D;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Check if player entered trigger
        if (collision.GetComponent<Player>())
        {
            AudioManager.instance.StopAreaThemeWithFade();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        boxCollider2D = GetComponent<BoxCollider2D>();
        Vector3 boxPos = boxCollider2D.transform.position;
        Vector2 vector = new Vector2(boxPos.x + boxCollider2D.offset.x, boxPos.y + boxCollider2D.offset.y);
        Gizmos.DrawWireCube(vector, GetComponent<BoxCollider2D>().size);
        Gizmos.DrawIcon(transform.position, "FMOD/FMODEmitter.tiff");
    }
}
