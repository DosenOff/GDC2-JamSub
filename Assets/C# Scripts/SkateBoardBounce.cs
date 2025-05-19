using UnityEngine;

public class SkateBoardBounce : MonoBehaviour
{
    public float bounceMultiplier = 1f;

    Player player;

    void Start()
    {
        player = GetComponentInParent<Player>();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Vector2 inVelocity = player.rb.linearVelocity;
        Vector2 normal = other.contacts[0].normal;
        Vector2 reflected = Vector2.Reflect(inVelocity, normal);

        player.rb.linearVelocity = reflected * bounceMultiplier;
    }
}
