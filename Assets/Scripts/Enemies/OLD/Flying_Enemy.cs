using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(Controller_2D))]
public class Flying_Enemy : MonoBehaviour
{
    Controller_2D controller;

    Player player;
    Transform playerTransfrom;
    public LayerMask viewMask;

    [SerializeField] private Vector2 attackPos;
    [SerializeField] private LayerMask playerMask;

    [SerializeField] private float lookRadius;
    [SerializeField] private float attackRange;

    [SerializeField] private float waitTime;

    float Health;
    [SerializeField] private float maxHealth;

    [SerializeField] private Vector2 knockback;

    [HideInInspector]
    [SerializeField] private Vector3 velocity;

    RaycastHit2D hit;

    SpriteRenderer sprite;
    public float damageDealt;

    Animator anim;

    [SerializeField] private Canvas canvas;
    [SerializeField] private Slider healthSlider;

    public bool isAggro = false;
    public bool inRange = false;
    public bool isDead = false;
    private readonly float flashTime = 0.3f;

    public int coinDrop;
    public GameObject coinPrefab;

    public GameManager gm;

    public Vector2 flyPos;
    public float flyRange;

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
    }

    void Start()
    {
        flyPos = transform.position;
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        player = gm.player.GetComponent<Player>();
        playerTransfrom = player.transform;
        Health = maxHealth;
        controller = GetComponent<Controller_2D>();
        healthSlider.value = CalculateHealthPercent();
        canvas.enabled = false;
    }

    private void Update()
    {
        InAttackRange();
        if (isAggro && !isDead)
        {
            anim.SetBool("isChasing", true);
            anim.SetBool("isPatroling", false);
        }
        else
        {
            anim.SetBool("isChasing", false);
            anim.SetBool("isPatroling", true);
        }
        if (inRange && !isDead)
        {
            anim.SetBool("Shoot", true);
        }
        else
        {
            anim.SetBool("Shoot", false);
        }

    }

    public void CoinSpawner()
    {
        for (int i = 0; i < coinDrop; i++)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }
    }

    public void InAttackRange()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(new Vector2(transform.position.x + (attackPos.x * transform.localScale.x), transform.position.y + (attackPos.y * transform.localScale.y)), attackRange, playerMask);
        inRange = (playerCollider != null);
    }

    public void TakeDamage(int dmg)
    {
        canvas.enabled = true;
        isAggro = true;
        velocity += 20 * Vector3.Normalize(transform.position - player.transform.position);
        Health -= dmg;
        healthSlider.value = CalculateHealthPercent();
        //StartCoroutine(Flash());
        //sprite.material.color = Color.Lerp(colorStart, colorEnd, Mathf.PingPong(.5f, 1));
        anim.SetTrigger("Hit");
        if (Health <= 0)
        {
            StartCoroutine(Die());
        }
    }
    private IEnumerator Die()
    {
        isDead = true;
        anim.SetBool("isDead", true);
        CoinSpawner();
        gameObject.GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        Destroy(gameObject);
    }
    /*
    public IEnumerator Flash()
    {
        sprite.material.color = colorEnd;
        yield return new WaitForSeconds(flashTime);
        sprite.material.color = colorStart;
    }
    */
    private float CalculateHealthPercent()
    {
        return Health / maxHealth;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector2(transform.position.x + (attackPos.x * transform.localScale.x), transform.position.y + (attackPos.y * transform.localScale.y)), attackRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(flyPos, flyRange);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.isTrigger != true && collision.tag == "Player")
        {
            //player.DealDamage(damageDealt, Vector3.Normalize(player.transform.position - transform.position));
        }
    }
}
