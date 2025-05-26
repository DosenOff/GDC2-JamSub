using System.Collections;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    public Vector3 target;
    public float roamRadius = 50f;

    public float speed = 200f;
    public float chaseSpeed = 2000f;
    private float savedSpeed;

    public float nextWaypointDistance = 3f;

    Path path;
    int currentWaypoint = 0;
    [SerializeField] private bool newPath = true;

    bool beginning = false;

    Seeker seeker;
    [HideInInspector] public Rigidbody2D rb;
    Player player;

    GuardGun guardGun = null;
    bool hasGun = false;
    [HideInInspector] public bool detected = false;

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, roamRadius);
    }

    void Start()
    {
        savedSpeed = speed;

        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player").GetComponent<Player>();

        foreach (Transform child in transform)
        {
            if (child.name == "Gun")
            {
                hasGun = true;
                guardGun = gameObject.GetComponentInChildren<GuardGun>();
            }
        }

        StartCoroutine(FindNewRoamPosition());

        if (beginning)
            InvokeRepeating("UpdatePath", 0f, .5f);
    }

    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            if (hasGun && detected)
            {
                speed = chaseSpeed;
                target = player.transform.position;
            }

            else
            {
                speed = savedSpeed;
                StartCoroutine(FindNewRoamPosition());
            }

            seeker.StartPath(rb.position, target, OnPathComplete);
        }
    }

    IEnumerator FindNewRoamPosition()
    {
        while (newPath)
        {
            Vector2 newRandomPos = Random.insideUnitCircle * roamRadius;

            ///modify this to equal the diameter in A* script
            Collider2D hit = Physics2D.OverlapCircle(newRandomPos, 3.5f);

            if (hit == null)
            {
                ///only for start so it doesn't move to (0, 0, 0)
                beginning = true;

                newPath = false;
                target = newRandomPos;
                yield break;
            }
        }

        yield return null;
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    void FixedUpdate()
    {
        if (path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            newPath = true;
            return;
        }

        else
        {
            newPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;
        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
            currentWaypoint++;
    }
}
