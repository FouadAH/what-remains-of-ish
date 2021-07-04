using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillArea : MonoBehaviour {

    private Player player;
    public int damageDealt;
    private GameManager gm;

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
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
}
