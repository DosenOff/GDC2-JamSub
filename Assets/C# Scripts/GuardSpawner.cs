using UnityEngine;

public class GuardSpawner : MonoBehaviour
{
    public GameObject guard;
    public float spawnTimer;
    public int maxGuardCount = 5;
    private float maxSpawnTimer;
    public int initialSpawnAmount = 4;

    private Collider2D col;
    private EnemyAI enemyAI;

    public int guardCount = 0;

    void Start()
    {
        enemyAI = guard.GetComponent<EnemyAI>();
        col = GetComponent<PolygonCollider2D>();
        maxSpawnTimer = spawnTimer;

        for (int i = 0; i < initialSpawnAmount; i++)
        {
            Spawn();
        }
    }

    public void Spawn()
    {
        Bounds bounds = col.bounds;

        for (int i = 0; i < 999; i++)
        {
            Vector2 randomPos = new Vector2(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y));

            if (col.OverlapPoint(randomPos))
            {
                Instantiate(guard, randomPos, Quaternion.identity);
                return;
            }
        }
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f && guardCount < maxGuardCount)
        {
            spawnTimer = maxSpawnTimer;
            Spawn();
        }
    }
}
