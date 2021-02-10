using UnityEngine;
using Pathfinding;
using System;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]
public class DroneAI : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    public float updateRate = 2f;

    private Rigidbody2D rb;
    private Seeker seeker;

    public Path path;

    public float speed = 300f;
    public ForceMode2D fMode;

    [HideInInspector] public bool pathIsEnded = false;

    public float nextWaypointDistance = 3f;
    private int currentWaypoint = 0;

    [SerializeField] Player_Input playerInput;

    private void Awake()
    {
        //GameManager.instance.drone = gameObject;
    }

    private void Start()
    {
        GameManager.instance.drone = gameObject;
        transform.position = GameManager.instance.dronePosition;
        target = GameManager.instance.player.transform;
        playerInput = GameManager.instance.player.GetComponent<Player_Input>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        if(target == null)
        {
            Debug.Log("No Player found");
            return;
        }

        seeker.StartPath(transform.position, target.position - offset, OnPathComplete);
        StartCoroutine(UpdatePath());
    }

    void FixedUpdate()
    {
        if (target == null)
        {
            //TODO: Player Search
            Debug.Log("No Player found");
            return;
        }

        if (path == null)
            return;

        if(currentWaypoint >= path.vectorPath.Count)
        {
            if (pathIsEnded)
                return;


            pathIsEnded = true;
            return;
        }

        pathIsEnded = false;

        //Direction to the next waypoint    
        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        dir *= speed * Time.fixedDeltaTime;

        float distToTarget = Vector3.Distance(transform.position, target.position - offset);
        if (distToTarget < 2f)
            return;

        //Move AI
        rb.AddForce(dir, fMode);

        float dist = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
        if (dist < nextWaypointDistance)
        {
            currentWaypoint++;
            return;
        }
    }

    void Update()
    {
        if (playerInput.PS4_Controller == 1  || playerInput.Xbox_One_Controller == 1)
        {
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(Input.GetAxisRaw("RHorizontal") * -1, Input.GetAxisRaw("RVertical")) * Mathf.Rad2Deg);
        }
        else
        {
            var pos = Camera.main.WorldToScreenPoint(transform.position);
            var dir = Input.mousePosition - pos;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }
        
    }

    public void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    IEnumerator UpdatePath()
    {
        if(target == null)
        {
            //TODO: Player Search
            Debug.Log("No Player found");
            yield return false;
        }

        float distToTarget = Vector3.Distance(transform.position, target.position - offset);
        if (distToTarget < 2f)
            yield return false;

        seeker.StartPath(transform.position, target.position - offset, OnPathComplete);
        yield return new WaitForSeconds(1f/updateRate);
        StartCoroutine(UpdatePath());

    }
    
}
