using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour, IDamageable
{
    public int score = 1000;

    [Header("Health")]
    public int health = 100;
    public HealthBar healthBar;
    public GameObject canvas;

    public SpriteRenderer[] skin;
    public float flashDuration = 0.1f;
    public GameObject deathEffect;

    [Header("Movement")]
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

    private GuardSpawner guardSpawner;

    public AudioClip deathSound;
    private AudioSource audioSource;

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, roamRadius);
    }

    void Start()
    {
        audioSource = GameObject.Find("SFXAudioSource").GetComponent<AudioSource>();

        guardSpawner = GameObject.Find("GuardSpawner").GetComponent<GuardSpawner>();
        guardSpawner.guardCount++;

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
            Vector2 newRandomPos = (Vector2)transform.position + Random.insideUnitCircle * roamRadius;

            ///modify this to equal the diameter in A* script
            Collider2D[] hits = Physics2D.OverlapCircleAll(newRandomPos, 3.5f);

            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Spawner") || hits == null)
                {
                    ///only for start so it doesn't move to (0, 0, 0)
                    beginning = true;

                    newPath = false;
                    target = newRandomPos;
                    yield break;
                }
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

    private void Update()
    {
        healthBar.SetHealth(health);;
    }

    public void TakeDamage(int damage)
    {
        canvas.SetActive(true);
        health -= damage;
        FlashWhite();

        if (health <= 0)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            player.AddScore(score);
            guardSpawner.guardCount--;
            audioSource.PlayOneShot(deathSound);
            Destroy(gameObject);
        }
    }

    void FlashWhite()
    {
        List<Color> savedColors = new List<Color>();

        foreach (var spriteRenderer in skin)
        {
            savedColors.Add(spriteRenderer.color);
            spriteRenderer.color = Color.white;
        }

        StartCoroutine(Flash(savedColors));
    }

    IEnumerator Flash(List<Color> savedColors)
    {
        yield return new WaitForSeconds(flashDuration);

        for (int i = 0; i < skin.Length; i++)
        {
            skin[i].color = savedColors[i];
        }
    }
}
