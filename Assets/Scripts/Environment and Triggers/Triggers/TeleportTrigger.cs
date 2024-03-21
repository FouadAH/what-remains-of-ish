using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeleportTrigger : MonoBehaviour
{
    public float cooldownTime = 1f;
    public Color inactiveColor = Color.grey;
    public Color activeColor = Color.red;

    SpriteRenderer spriteRenderer;
    bool isInactive = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Boomerang>() && !isInactive) 
        {
            GameObject player = GameManager.instance.player;
            player.GetComponent<PlayerTeleport>().TeleportAbility(player.transform);
            player.GetComponent<PlayerTeleport>().ResetTeleport();
            player.GetComponent<PlayerDash>().ResetDash();
            StartCoroutine(CooldownTimer());
        }
    }

    IEnumerator CooldownTimer()
    {
        isInactive = true;
        spriteRenderer.color = inactiveColor;
        yield return new WaitForSeconds(cooldownTime);
        spriteRenderer.color = activeColor;
        isInactive = false;
    }
}
