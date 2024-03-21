using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class BendingGrass : MonoBehaviour
{

    public float bendFactor;
    public Rigidbody2D baseRigidbody;
    Vector2 bendForce;
    Vector2 bendForceVelocity;
    bool isBending;
    bool isRebounding;

    SpriteSkin skin;
    SpriteRenderer spriteRenderer;
    Sprite sprite;
    BoxCollider2D boxCollider;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        sprite = spriteRenderer.sprite;
        boxCollider = GetComponent<BoxCollider2D>();
        skin = GetComponent<SpriteSkin>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (!isBending)
        //{
        //    bendForce = Vector2.SmoothDamp(bendForce, Vector2.zero, ref bendForceVelocity, 0.5f);
        //}

        //baseRigidbody.AddForce(bendForce);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            float enterOffset = col.transform.position.x - transform.position.x;
            bendForce = new Vector3(bendFactor * -Mathf.Sign(enterOffset), 0, 0);
            baseRigidbody.AddForce(bendForce, ForceMode2D.Impulse);
            isBending = true;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            float exitOffset = col.transform.position.x - transform.position.x;
            bendForce = new Vector3(bendFactor * -Mathf.Sign(exitOffset), 0, 0);
            baseRigidbody.AddForce(bendForce, ForceMode2D.Impulse);
            isBending = false;
        }
    }

    //void OnTriggerStay2D(Collider2D col)
    //{
    //    if (col.gameObject.CompareTag("Player"))
    //    {
    //        var offset = col.transform.position.x - transform.position.x;

    //        if (isBending || Mathf.Sign(enterOffset) != Mathf.Sign(offset))
    //        {
    //            isRebounding = false;
    //            isBending = true;

    //            // figure out how far we have moved into the trigger and then map the offset to -1 to 1.
    //            // 0 would be neutral, -1 to the left and +1 to the right.
    //            var radius = boxCollider.bounds.size.x/2 + col.bounds.size.x * 0.5f;
    //            exitOffset = Map(offset, -radius, radius, -1f, 1f);
    //            setVertHorizontalOffset(exitOffset);
    //        }
    //    }
    //}

    public float Map(float x, float in_min, float in_max, float out_min, float out_max)
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }

    // simple method to offset the top 2 verts of a quad based on the offset and BEND_FACTOR constant
    void setVertHorizontalOffset(float offset)
    {
        var verts = sprite.vertices;

        verts[1].x = 0.5f + offset * bendFactor / transform.localScale.x;
        verts[3].x = -0.5f + offset * bendFactor / transform.localScale.x;

        sprite.OverrideGeometry(verts, sprite.triangles);
    }

    private void OnBecameInvisible()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        skin.enabled = false;
    }

    private void OnBecameVisible()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        skin.enabled = true;
    }


}
