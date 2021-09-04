using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillArea : MonoBehaviour {

    private Player player;
    public int damageDealt;
    private GameManager gm;
    public BoxCollider2D col2D;
    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        col2D = gameObject.GetComponent<BoxCollider2D>();
    }
    private void Start()
    {
        col2D = gameObject.GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            player = collision.gameObject.GetComponent<Player>();

            if (player.enabled)
            {
                player.ModifyHealth(damageDealt);

                if (GameManager.instance.health > 0)
                {
                    GameManager.instance.SoftRespawn();
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(col2D.bounds.center, col2D.bounds.size);
    }
}
