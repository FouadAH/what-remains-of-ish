using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : MonoBehaviour, IDamagable
{
    public float Health { get; set; }
    public int MaxHealth { get; set; }
    public int knockbackGiven { get; set; }

    public GameObject wall;
    public GameObject shadow;

    SpriteRenderer wallSprite;
    Collider2D wallCollider;
    Animator shadowAnimator;

    private void Start()
    {
        MaxHealth = 3;
        Health = MaxHealth;

        shadowAnimator = GetComponentInChildren<Animator>();

        wallCollider = wall.GetComponent<Collider2D>();
        wallSprite = wall.GetComponent<SpriteRenderer>();
    }
    public void KnockbackOnDamage(int amount, float dirX, float dirY)
    {
        return;
    }

    public void ModifyHealth(int amount)
    {
        Health--;
        Quaternion quaternion = Quaternion.Euler(new Vector3(-90, 0, 0));
        if (Health == 0)
        {
            GetComponent<Collider2D>().enabled = false;
            wallCollider.enabled = false;
            wallSprite.enabled = false;
        }
    }


}
