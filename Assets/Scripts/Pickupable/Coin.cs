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

    public IntegerReference playerCurrency;
    /// <summary>
    /// Called when a coin is intantiated.
    /// </summary>
    void Start()
    {
        float xForce = Random.Range(sideForce,-sideForce);
        float yForce = Random.Range(upForce/2f, upForce);
        GetComponent<Rigidbody2D>().velocity = new Vector2(xForce, yForce);
    }

    /// <summary>
    /// If the player moves over coin, change the players currency and destroy this object 
    /// </summary>
    /// <param name="collider">Player game object</param>
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            playerCurrency.Value += value;
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Interactive Objects/Scraps", GetComponent<Transform>().position);
            Destroy(gameObject);
        }
    }

}
