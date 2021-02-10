using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour {

    private Player player;
    public int damageDealt;
    private GameManager gm;

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        player = gm.player.GetComponent<Player>();

        if (collision.gameObject.tag.Equals("Player"))
        {
            player = gm.player.GetComponent<Player>();
            player.ModifyHealth(damageDealt);
            player.Respawn();
        }
    }
}
