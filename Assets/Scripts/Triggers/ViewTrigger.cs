using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class ViewTrigger : MonoBehaviour {

    new CircleCollider2D collider;
    Enemy enemy;
    //[HideInInspector]
    public float size;

    public void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
        collider = GetComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = size;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.isTrigger && collision.tag == "Player")
        {
            enemy.SendMessage("Aggro");
        }
    }
}
