using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportRefill : MonoBehaviour
{
    public float resetTime = 3f;
    bool isActive = true;
    SpriteRenderer spriteRenderer;

    public Color activeColor;
    public Color inactiveColor;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = activeColor;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player)
        {
            Refill(player);
        }
    }

    public void Refill(Player player)
    {
        if (!isActive)
            return;

        PlayerTeleport playerTeleport = player.GetComponent<PlayerTeleport>();

        if (playerTeleport.teleportLockRoutine != null)
            playerTeleport.StopCoroutine(playerTeleport.teleportLockRoutine);

        playerTeleport.teleportLock = false;
        StartCoroutine(ResetTime());
        
    }

    IEnumerator ResetTime()
    {
        isActive = false;
        spriteRenderer.color = inactiveColor;
        yield return new WaitForSeconds(resetTime);
        isActive = true;
        spriteRenderer.color = activeColor;
    }

    void SetColor()
    {
        _ = (isActive) ? spriteRenderer.color = activeColor : spriteRenderer.color = inactiveColor;
    }

    
}
