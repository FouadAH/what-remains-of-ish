using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Coin logic and behavior
/// </summary>
public class Coin : MonoBehaviour
{
    [SerializeField] private float upForce = 3f;
    [SerializeField] private float sideForce = 3f;
    [SerializeField] private int value = 1;

    bool canPickUp = false;
    public Collider2D coinCollider;
    public ParticleSystem coinCollecterEffect;
    public IntegerReference playerCurrency;
    //public InventoryItemSO autoCollectBrooche;

    float waitTime = 0.2f;
    /// <summary>
    /// Called when a coin is intantiated.
    /// </summary>
    void Start()
    {
        float xForce = Random.Range(sideForce,-sideForce);
        float yForce = Random.Range(upForce/2f, upForce);
        GetComponent<Rigidbody2D>().velocity = new Vector2(xForce, yForce);
        coinCollider.enabled = false;
        StartCoroutine(Timer());

        //if (autoCollectBrooche.isEquipped)
        //{

        //}
    }

    /// <summary>
    /// If the player moves over coin, change the players currency and destroy this object 
    /// </summary>
    /// <param name="collider">Player game object</param>
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (canPickUp && IsInLayerMask(collider.gameObject.layer, LayerMask.GetMask("PlayerTrigger")))
        {
            playerCurrency.Value += value;
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Interactive Objects/Scraps", GetComponent<Transform>().position);
            Instantiate(coinCollecterEffect, transform.position, Quaternion.identity).Play();
            StopAllCoroutines();
            Destroy(gameObject);
        }

        if ( IsInLayerMask(collider.gameObject.layer, LayerMask.GetMask("Obstacles")) )
        {
            canPickUp = true;
            coinCollider.enabled = true;
        }
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(waitTime);
        canPickUp = true;
        coinCollider.enabled = true;
    }
    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }

}
