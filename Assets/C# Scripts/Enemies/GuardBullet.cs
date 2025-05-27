using UnityEngine;

public class GuardBullet : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;
    public GameObject bulletEffect;

    private Rigidbody2D rb;

    Player player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = transform.right * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player.TakeDamage(damage);
        }

        Instantiate(bulletEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
