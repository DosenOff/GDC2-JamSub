using UnityEngine;

public class GuardBullet : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 1f;

    private Rigidbody2D rb;

    Player player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = Vector2.right * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public float bulletSpeed()
    {
        return speed;
    }
}
