using UnityEngine;

public class BananaSpawner : MonoBehaviour
{
    public GameObject bananaObj;
    public float spawnTimer;
    public int initialSpawnAmount = 5;

    private Collider2D col;
    private Banana banana;

    void Start()
    {
        banana = bananaObj.GetComponent<Banana>();
        col = GetComponent<PolygonCollider2D>();
        spawnTimer = banana.maxDecayTime / 10f;

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

            Collider2D[] hits = Physics2D.OverlapCircleAll(randomPos, 1.5f);
            bool blocked = false;

            foreach (Collider2D hit in hits)
            {
                if (hit != col)
                {
                    blocked = true;
                    break;
                }
            }

            if (col.OverlapPoint(randomPos) && !blocked)
            {
                Instantiate(bananaObj, randomPos, Quaternion.identity);
                return;
            }
        }
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            spawnTimer = banana.maxDecayTime / 10f;
            Spawn();
        }
    }
}
