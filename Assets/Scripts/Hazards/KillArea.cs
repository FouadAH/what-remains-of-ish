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
                if (GameManager.instance.health > 1 && !GameManager.instance.isRespawning)
                {
                    player.ModifyHealth(damageDealt);
                    GameManager.instance.SoftRespawn();
                }
                else
                {
                    player.ModifyHealth(damageDealt);
                }
            }
        }else if (collision.gameObject.tag.Equals("Enemy"))
        {
            Entity enemy = collision.gameObject.GetComponent<Entity>();
            enemy.ModifyHealth(1000);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if(col2D == null)
            col2D = gameObject.GetComponent<BoxCollider2D>();

        Gizmos.DrawWireCube(col2D.bounds.center, col2D.bounds.size);
    }
}
